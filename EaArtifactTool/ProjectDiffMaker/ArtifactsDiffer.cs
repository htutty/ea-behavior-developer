using System;
using System.IO;
using System.Collections.Generic;

using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.writer;
using IndexAccessor;

namespace ProjectDiffMaker
{
	/// <summary>
	/// Description of ArtifactsDiffer.
	/// </summary>
	public class ArtifactsDiffer
	{
		public Boolean skipElementTaggedValue = true ;
		public Boolean skipElementTPosFlg = false ;
		public Boolean skipAttributeNoteFlg = false ;
		public Boolean skipMethodNoteFlg = false ;
		public Boolean skipAttributePosFlg = false ;
		public Boolean skipMethodPosFlg = false ;
		public Boolean outputDetailFileFlg = true ;

		private string fromProjectFile = null;
        private string fromProjectDir = null;
        private string fromArtifactDir = null;
        private string toProjectFile = null;
        private string toProjectDir = null;
        private string toArtifactDir = null;

        public string outputDir;
		
		private List<ArtifactVO> fromArtifacts;
		private List<ArtifactVO> toArtifacts;
		private List<ArtifactVO> outArtifacts;

        private List<PackageVO> fromPackageList;
        private List<PackageVO> toPackageList;
        // private List<PackageVO> outPackageList;

        private Dictionary<int, PackageVO> fromPackageMap = new Dictionary<int, PackageVO>();
        private Dictionary<int, PackageVO> toPackageMap = new Dictionary<int, PackageVO>();
        private Dictionary<int, PackageVO> outPackageMap = new Dictionary<int, PackageVO>();
        
        Dictionary<int, PackageVO> targetPackageMap;

        private ArtifactVO procArtifact = null;

		public ArtifactsDiffer() {
		}

		/// <summary>
		/// 比較元、比較先のプロジェクトを引数に取るコンストラクタ
		/// </summary>
		/// <param name="fromProjectFile_">比較元のプロジェクトファイル</param>
		/// <param name="toProjectFile_">比較先のプロジェクトファイル</param>
		public ArtifactsDiffer(string fromProjectFile, string toProjectFile, bool outputDetailFileFlg) {

            if ( fromProjectFile != null ) {
				this.fromProjectFile = fromProjectFile;
                this.fromProjectDir = Path.GetDirectoryName(fromProjectFile);
                this.fromArtifactDir = this.fromProjectDir + "\\artifacts";
            }
			
			if ( toProjectFile != null ) {
				this.toProjectFile = toProjectFile;
                this.toProjectDir = Path.GetDirectoryName(toProjectFile);
                this.toArtifactDir = this.toProjectDir + "\\artifacts";
            }

            this.outputDetailFileFlg = outputDetailFileFlg;

			Console.WriteLine("fromdir = " + this.fromArtifactDir);
			Console.WriteLine("todir = " + this.toArtifactDir);
		}
		

		/// <summary>
		/// 比較元、比較先プロジェクトの全成果物リストを読み込み
		/// </summary>
		public void readBothArtifacts() {
            Console.WriteLine("readAllArtifacts(): from");
            ProjectSetting.load(this.fromProjectFile);
            this.fromArtifacts = readAllArtifacts(this.fromArtifactDir);

            string allPackagesFileName = this.fromProjectDir + "\\AllPackageTree.xml";
            PackagesXmlReader fromPacReader = new PackagesXmlReader(allPackagesFileName);
            this.fromPackageList = fromPacReader.getAllPackages();
            this.fromPackageMap = storePackageMapFromList(this.fromPackageList);


            Console.WriteLine("readAllArtifacts(): to");
            ProjectSetting.load(this.toProjectFile);
            this.toArtifacts = readAllArtifacts(this.toArtifactDir);

            allPackagesFileName = this.toProjectDir + "\\AllPackageTree.xml";
            PackagesXmlReader toPacReader = new PackagesXmlReader(allPackagesFileName);
            this.toPackageList = toPacReader.getAllPackages();
            this.toPackageMap = storePackageMapFromList(this.toPackageList);
        }


        /// <summary>
        /// Packageリストの内容をPackegeマップに移す。
        /// </summary>
        /// <param name="inList"></param>
        /// <returns></returns>
        private Dictionary<int, PackageVO> storePackageMapFromList(List<PackageVO> inList)
        {
            Dictionary<int, PackageVO> outMap = new Dictionary<int, PackageVO>() ;
            retrieveList(inList, outMap);

            return outMap;
        }


        /// <summary>
        /// PackageMapに移すにあたり、パッケージツリーで再帰的にリストをなめる。
        /// </summary>
        /// <param name="inList"></param>
        /// <returns></returns>
        private void retrieveList(List<PackageVO> inList, Dictionary<int, PackageVO> outMap) 
        {
            // 
            foreach( PackageVO pac in inList )
            {
                outMap.Add(pac.packageId, new PackageVO(pac));
                retrieveList(pac.childPackageList, outMap);
            }

        }


        /// <summary>
        /// プロジェクト内の全成果物の読み込み
        ///  AllArtifacts.xml → atf_xxxx.xml ファイルを読み、全ての成果物パッケージの内容をメモリに読み込む
        /// </summary>
        /// <param name="projectDir"></param>
        /// <returns>全成果物のリスト</returns>
        private List<ArtifactVO> readAllArtifacts( string artifactsDir) {
            List<ArtifactVO> retList = ArtifactsXmlReader.readArtifactList(artifactsDir, ProjectSetting.getVO().artifactsFile);
			ArtifactXmlReader atfReader = new ArtifactXmlReader(artifactsDir);
			
			foreach( ArtifactVO atf in retList ) {
				// 成果物パッケージ別のXMLファイル読み込み
				atfReader.readArtifactDesc(atf);

                // 成果物配下の子ノードをGUIDでソート
                atf.sortChildNodesGuid();
            }

            // 成果物リストをソートする。 ソートキー＝GUID（自然順序付け）
            // see: ArtifactAccessor.vo.ArtifactVO#compareTo
			retList.Sort();
			return retList;
		}
		
		
		/// <summary>
		/// 全成果物のマージ
		/// </summary>
		public void mergeAllArtifacts() {
			Int16 lCnt, rCnt;
			ArtifactVO lAtf, rAtf, outAtf;
			List<ArtifactVO> outList = new List<ArtifactVO>();

			for (lCnt=0, rCnt=0; lCnt < fromArtifacts.Count || rCnt < toArtifacts.Count; ) {
				lAtf = fromArtifacts[lCnt];
				rAtf = toArtifacts[rCnt];
				
				// GUID比較で一致した場合: 成果物のパッケージ内の内容比較に移行する
				if (compareArtifactGuid(lAtf, rAtf) == 0) {
					// 現在処理対象の成果物を保持する
					procArtifact = lAtf;
					outAtf = getAgreedContentsOfArtifact(lAtf, rAtf);

                    if( outAtf.changed != ' ' )
                    {
                        outList.Add(outAtf);
                    }
                    lCnt++;
					rCnt++;
				} else {
					// GUID比較で一致しなかった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
					if(compareArtifactGuid(lAtf, rAtf) > 0) {
						// 現在処理対象の成果物を保持する
						procArtifact = rAtf;
						outAtf = rAtf;
						outAtf.changed = 'C';
						
						setChangedToChilds(outAtf.package, 'C');
						outList.Add(outAtf);
						rCnt++;
					} else {
						// 現在処理対象の成果物を保持する
						procArtifact = lAtf;
						outAtf = lAtf;
						outAtf.changed = 'D';
						
						setChangedToChilds(outAtf.package, 'D');
						outList.Add(outAtf);
						lCnt++;
					}
				}
			}
			
			this.outArtifacts = outList;
		}

        /// <summary>
        /// 成果物の中での同一GUIDのパッケージの中身の比較
        /// </summary>
        /// <param name="leftAtf">成果物：左</param>
        /// <param name="rightAtf">成果物：右</param>
        /// <returns></returns>
        private ArtifactVO getAgreedContentsOfArtifact(ArtifactVO leftAtf, ArtifactVO rightAtf) {
			ArtifactVO outAtf = leftAtf;
			
			outAtf.package = getDisagreedPackage(leftAtf.package, rightAtf.package) ;
			if (outAtf.package == null ) {
				outAtf.changed = ' ';
			} else {
				outAtf.changed = 'U';
			}
			return outAtf;
		}
		
		/// <summary>
        /// パッケージ配下のマージ
        /// </summary>
        /// <param name="leftPkg"></param>
        /// <param name="rightPkg"></param>
        /// <returns></returns>
		private PackageVO getDisagreedPackage(PackageVO leftPkg, PackageVO rightPkg) {
			PackageVO outPkg;
			if( !leftPkg.name.Equals(rightPkg.name) ) {
				outPkg = rightPkg;
				outPkg.changed = 'U';
			} else {
				outPkg = leftPkg;
				outPkg.changed = ' ';
			}

			Int16 lCnt, rCnt;

			// compare Contents of child Package
			PackageVO lPkg, rPkg, oPkg;
			List<PackageVO> outPackageList = new List<PackageVO>();
			for (lCnt=0, rCnt=0; lCnt < leftPkg.childPackageList.Count || rCnt < rightPkg.childPackageList.Count; ) {
				// 左側が最終の要素に達した場合
				if( lCnt >= leftPkg.childPackageList.Count ) {
					// 右側のパッケージが追加されたものとして出力パッケージリストに追加
					oPkg = rightPkg.childPackageList[rCnt];
					oPkg.changed = 'C';
					
					setChangedToChilds(oPkg, 'C');
					outPackageList.Add(oPkg);
					rCnt++;
					continue;
				}

				// 右側が最終の要素に達した場合
				if( rCnt >= rightPkg.childPackageList.Count ) {
					// 左側のパッケージが削除されたものとして出力パッケージリストに追加
					oPkg = leftPkg.childPackageList[lCnt];
					oPkg.changed = 'D';

					setChangedToChilds(oPkg, 'D');
					outPackageList.Add(oPkg);
					lCnt++;
					continue;
				}

				
				lPkg = leftPkg.childPackageList[lCnt];
				rPkg = rightPkg.childPackageList[rCnt];
				
				// GUIDの比較で一致した場合
				if (comparePackageGuid(lPkg, rPkg) == 0) {
					oPkg = getDisagreedPackage(lPkg, rPkg);
					if (oPkg != null) {
						oPkg.changed = 'U';
						outPackageList.Add(oPkg);
					}
					lCnt++;
					rCnt++;
				} else {
					// 比較で一致するものが無かった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
					if(comparePackageGuid(lPkg, rPkg) > 0) {
						oPkg = rPkg;
						oPkg.changed = 'C';
						
						setChangedToChilds(oPkg, 'C');
						outPackageList.Add(oPkg);
						rCnt++;
					} else {
						oPkg = lPkg;
						oPkg.changed = 'D';
						
						setChangedToChilds(oPkg, 'D');
						outPackageList.Add(oPkg);
						lCnt++;
					}
				}

				
				Console.Write("■子Package比較: 左=" + lCnt + ", " + lPkg.name + "[" + lPkg.guid + "]" );
				Console.WriteLine(" 右=" + rCnt + ", " + rPkg.name + "[" + rPkg.guid + "]" );

				if ( oPkg == null ) {
					Console.WriteLine("　→　結果: 一致"  );
				} else {
					Console.WriteLine("　→　結果: 不一致=" + oPkg.changed );
				}
				
			}
			outPkg.childPackageList = outPackageList;

			getAgreedPackageContents(outPkg, leftPkg.elements, rightPkg.elements);
			
			if (outPkg.elements.Count == 0 && outPkg.childPackageList.Count == 0) {
				if( outPkg.changed == ' ') {
					return null;
				} else {
					outPkg.changed = 'U';
					return outPkg;
				}
			} else {
				outPkg.changed = 'U';
				return outPkg;
			}
			
		}


		/// <summary>
		/// パッケージが保持する要素リストの比較
		/// </summary>
		/// <param name="outPkg"></param>
		/// <param name="lCnt"></param>
		/// <param name="rCnt"></param>
		/// <param name="leftPkg"></param>
		/// <param name="rightPkg"></param>
		void getAgreedPackageContents(PackageVO outPkg, List<ElementVO> leftElements, List<ElementVO> rightElements)
		{
			Int16 lCnt, rCnt;

			ElementDiffer differ = new ElementDiffer();
			differ.skipAttributeNoteFlg = this.skipAttributeNoteFlg;
			differ.skipElementTaggedValue = this.skipElementTaggedValue;
			differ.skipElementTPosFlg = this.skipElementTPosFlg;
			differ.skipAttributeNoteFlg = this.skipAttributeNoteFlg;
			differ.skipAttributePosFlg = this.skipAttributePosFlg;
			differ.skipMethodNoteFlg = this.skipMethodNoteFlg;
			differ.skipMethodPosFlg = this.skipMethodPosFlg;
			
			
		    // GUIDで一致するものがあった場合:

		    // GUIDで一致するものが無かった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
			ElementVO lElm, rElm, oElm;
			List<ElementVO> outElements = new List<ElementVO>();
			for (lCnt = 0,rCnt = 0; lCnt < leftElements.Count || rCnt < rightElements.Count;) {

				// 左側が最終の要素に達した場合
				if (lCnt >= leftElements.Count) {
					// 右側の要素が追加されたものとして出力パッケージリストに追加
					oElm = rightElements[rCnt];
					oElm.changed = 'C';
					outElements.Add(oElm);
					rCnt++;
					continue;
				}

				// 右側が最終の要素に達した場合
				if (rCnt >= rightElements.Count) {
					// 左側の要素が削除されたものとして出力パッケージリストに追加
					oElm = leftElements[lCnt];
					oElm.changed = 'D';
					outElements.Add(oElm);
					lCnt++;
					continue;
				}
				
				lElm = leftElements[lCnt];
				rElm = rightElements[rCnt];


				if (compareElementGuid(lElm, rElm) == 0) {
					// 要素の内容を比較
					oElm = differ.getDisagreedElement(lElm, rElm);
					if (oElm != null) {
						oElm.changed = 'U';
						outElements.Add(oElm);
					}
					lCnt++;
					rCnt++;
				} else {
					if (compareElementGuid(lElm, rElm) > 0) {
						rElm.changed = 'C';
						outElements.Add(rElm);
						rCnt++;
					} else {
						lElm.changed = 'D';
						outElements.Add(lElm);
						lCnt++;
					}
				}
			}
			outPkg.elements = outElements;
		}

        /// <summary>
        /// パッケージ単位での追加(C)・削除(D)があったら配下の要素と、さらに属性・メソッドに伝搬する
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="updChanged"></param>
        private void setChangedToChilds(PackageVO pkg, char updChanged) {
			pkg.changed = updChanged;
			foreach( PackageVO p in pkg.childPackageList ) {
				setChangedToChilds(p, updChanged);
			}
			
			foreach( ElementVO e in pkg.elements ) {
				e.changed = updChanged;
				
				foreach ( AttributeVO a in e.attributes) {
					a.changed = updChanged;
				}
				
				foreach ( MethodVO m in e.methods) {
					m.changed = updChanged;
				}
				
			}

			return;
		}

		
		/// <summary>
		/// マージ時に使用：GUIDによる成果物の比較
		/// </summary>
		/// <param name="leftAtf">左の成果物</param>
		/// <param name="rightAtf">右の成果物</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private static int compareArtifactGuid(ArtifactVO leftAtf, ArtifactVO rightAtf) {
			return leftAtf.guid.CompareTo(rightAtf.guid);
		}
		
		/// <summary>
		/// マージ時に使用：GUIDによるパッケージの比較
		/// </summary>
		/// <param name="leftAtf">左のパッケージ</param>
		/// <param name="rightAtf">右のパッケージ</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private static int comparePackageGuid(PackageVO leftPkg, PackageVO rightPkg) {
			return leftPkg.guid.CompareTo(rightPkg.guid);
		}
				
		/// <summary>
		/// マージ時に使用：GUIDによる要素の比較
		/// </summary>
		/// <param name="leftAtf">左の要素</param>
		/// <param name="rightAtf">右の要素</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private static int compareElementGuid(ElementVO leftElm, ElementVO rightElm) {
			return leftElm.guid.CompareTo(rightElm.guid);
		}

		/// <summary>
        /// 成果物ごとの差異の有無をコンソール出力
        /// </summary>
        /// <param name="atf"></param>
		private static void outputConsole( ArtifactVO atf ) {
			switch( atf.changed ) {
				case 'U':
					Console.WriteLine("更新: GUID="+ atf.guid + ", Name=" + atf.name );
					break;
				case 'C':
					Console.WriteLine("追加: GUID="+ atf.guid + ", Name=" + atf.name );
					break;
				case 'D':
					Console.WriteLine("削除: GUID="+ atf.guid + ", Name=" + atf.name );
					break;
				default:
					Console.WriteLine("不明: GUID="+ atf.guid + ", Name=" + atf.name);
					break;
			}

		}


		/// <summary>
        /// マージ済みの成果物情報を各ファイルに出力する
        /// </summary>
		public void outputMerged() {
			Console.WriteLine("outputMerged: outputDir=" + this.outputDir);

            // 変更後XMLファイルを出力する
            ChangedArtifactXmlWriter changedWriter = new ChangedArtifactXmlWriter();
            changedWriter.writeChagedArtifacts(this.outputDir, this.outArtifacts);

            // CSVファイル（変更サマリー、詳細）を出力する
            CsvReportWriter csvWriter = new CsvReportWriter();
            csvWriter.writeSummaryCsvFile(this.outputDir, this.outArtifacts);
            csvWriter.writeDetailCsvFile(this.outputDir, this.outArtifacts);

            // 何らかの差分があった分だけのパッケージツリーを生成し、XMLに出力
            doOutputPackagesXml(this.outArtifacts);

            // elementsフォルダ配下に要素を出力する(BehaviorDeveloperで見る用)
            doMakeIndex(this.outArtifacts);
            doMakeElementFiles(this.outArtifacts);

            // 明細ファイル出力フラグが立っていなければそのままリターン
            if (outputDetailFileFlg)
            {
                // 変更前後のdetailファイルを出力する
                DetailXmlWriter dtlWriter = new DetailXmlWriter(this.fromArtifactDir, this.toArtifactDir);
                dtlWriter.writeDetailFiles(this.outputDir, this.outArtifacts);
            }

        }

        /// <summary>
        /// 差分のあったパッケージの分でパッケージXMLを出力する。
        /// </summary>
        /// <param name="outArtifacts"></param>
        private void doOutputPackagesXml(List<ArtifactVO> outArtifacts)
        {
            bool throughFlg;
            List<PackageVO> rootPackages = new List<PackageVO>();

            // 出力用の成果物リストを読み込み
            foreach ( ArtifactVO atf in outArtifacts)
            {
                throughFlg = false;
                switch (atf.changed)
                {
                    case 'U':
                        this.targetPackageMap = this.toPackageMap;
                        break;
                    case 'C':
                        this.targetPackageMap = this.toPackageMap;
                        break;
                    case 'D':
                        this.targetPackageMap = this.fromPackageMap;
                        break;
                    default:
                        // throw new Exception("変更区分が不正です : '" + atf.changed + "'");
                        throughFlg = true;
                        break;
                }

                if( !throughFlg )
                {
                    addPackageToOutList(atf.package, rootPackages);
                }
            }

            PackagesXmlWriter writer = new PackagesXmlWriter(this.outputDir);
            writer.outputPackageXml(rootPackages);
        }

        /// <summary>
        /// 指定された成果物パッケージを出力用パッケージツリーに追加する
        /// </summary>
        /// <param name="atfPac"></param>
        /// <param name="rootPackages"></param>
        private void addPackageToOutList(PackageVO atfPac, List<PackageVO> rootPackages)
        {

            // 成果物パッケージIDが取得元のパッケージMapに含まれている場合
            if (this.targetPackageMap.ContainsKey(atfPac.packageId))
            {
                // 対象のパッケージMapから、成果物パッケージのインスタンスを取得
                // PackageVO atfpac = this.targetPackageMap[atfPac.packageId];

                // ルートから成果物パッケージに辿り着くためのパッケージを親から子の順番で並べたリストを取得
                List<PackageVO> packages = traverseFromLeafToRoot(atfPac);

                // リストの個数が 0 より大きい場合
                if (packages.Count > 0)
                {
                    // 成果物パッケージの階層ごとのパッケージIDのリストの0番目（ルート）を取得
                    PackageVO rootPackage = packages[0];
                    int rootPackageId = packages[0].packageId;

                    // パッケージMapに該当のパッケージIdが存在していなければ
                    if (!outPackageMap.ContainsKey(rootPackageId))
                    {
                        // ルートPackageを生成し返却値のPackageリストに追加
                        rootPackages.Add(rootPackage);
                        outPackageMap.Add(rootPackageId, rootPackage);

                        // ２階層目以降
                        concatChildNodes(packages, 1, atfPac, rootPackage);
                    }
                    // パッケージMapに該当のパッケージIdが存在していたら
                    else
                    {
                        // 既にルート以降のパッケージ構造が存在しているはずなので追加するべきパッケージを探し、
                        // 返却値のPackageリスト内にツリー構造を追加する
                        searchAndConcatChildNodes(packages, 0, atfPac, null, rootPackages);
                    }

                }
                
            }
            else
            {
                Console.WriteLine("PackageId が不正です : " + atfPac.packageId);
                // throw new Exception("PackageId が不正です : " + atfPac.packageId);
            }
        }


        /// <summary>
        /// 引数のパッケージリストを１階層ずつ指定の親パッケージの子として追加する
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="idx"></param>
        /// <param name="parentPackage"></param>
        private void concatChildNodes(List<PackageVO> packages, int idx, PackageVO artifactPackage, PackageVO parentPackage)
        {
            if( idx >= packages.Count )
            {
                return;
            }

            PackageVO myPackage;

            // リスト内の最後の１件は成果物パッケージ(isControlledがtrue)なので、引数の成果物パッケージの方を追加する
            if ( idx == packages.Count -1 )
            {
                myPackage = artifactPackage;
            }
            else
            {
                myPackage = packages[idx];
            }
            
            parentPackage.childPackageList.Add(myPackage);

            if(!outPackageMap.ContainsKey(myPackage.packageId))
            {
                outPackageMap.Add(myPackage.packageId, myPackage);
            }
            else
            {
                Console.WriteLine("PackageIdがOutPackageMap内で重複しました : " + myPackage.packageId);
            }

            concatChildNodes(packages, idx + 1, artifactPackage, myPackage);
        }


        /// <summary>
        /// 引数のPackageリストを、ルートから目的のパッケージまでの経路と見立て、
        /// 先頭から階層順に出力パッケージツリー内の存在チェックを行い、存在しない階層が来たらそれ以降を出力パッケージツリーに加える
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="idx"></param>
        /// <param name="parentPackage"></param>
        /// <param name="targetPackages"></param>
        private void searchAndConcatChildNodes(List<PackageVO> packages, int idx, PackageVO artifactPackage, PackageVO parentPackage, List<PackageVO> targetPackages)
        {
            if (idx >= packages.Count)
            {
                return;
            }

            PackageVO myPackage = packages[idx];
            int myPackageId = myPackage.packageId;
            bool foundFlg = false;

            foreach ( PackageVO ppac in targetPackages)
            {
                // parentPackagesの中に自分のIDが存在している場合、
                // 対象の階層で一致している間は階層を下げて自メソッドを再帰呼び出しする
                if (ppac.packageId == myPackageId)
                {
                    searchAndConcatChildNodes(packages, idx+1, artifactPackage, ppac, ppac.childPackageList);
                    foundFlg = true;
                    break;
                }

            }

            // 同レベルに見つからなかったということは、これ以降のパッケージは全て追加でよいので
            // 以降は単純な追加処理を呼ぶ
            if( !foundFlg )
            {
                concatChildNodes(packages, idx, artifactPackage, parentPackage);
            }

        }

        /// <summary>
        /// 指定したパッケージをparentPackageIdを使って先頭（ルート）まで辿り、
        /// 辿った結果としてルートから子への順でパッケージIDのリストを作成し返却する
        /// 
        /// <pre>
        /// id=1, root
        ///   id=2, xxx
        ///   id=3, yyy
        ///     id=4, zzz
        /// </pre>
        /// というパッケージ構造があり、このうち id=4 の zzz が指定されると、
        /// 返却値は 1,3,4 という3つのパッケージIDのパッケージが格納されたリストとなる
        /// 
        /// </summary>
        /// <param name="atfpac"></param>
        /// <returns></returns>
        private List<PackageVO> traverseFromLeafToRoot(PackageVO atfpac)
        {
            List<PackageVO> outList = new List<PackageVO>();

            goBackRetrievePackage(atfpac, outList);
            return outList;
        }


        /// <summary>
        /// 下から上にパッケージを辿る。
        /// 返却リストは階層の上から順にパッケージが並ぶように、先頭に追加する
        /// </summary>
        /// <param name="pac"></param>
        /// <param name="outList"></param>
        private void goBackRetrievePackage(PackageVO pac, List<PackageVO> outList)
        {
            // 返却値となるパッケージのリスト
            outList.Insert(0, pac);

            // 親パッケージIDが０になったら終了
            if ( pac.parentPackageId > 0 )
            {
                // 親パッケージIDから親が辿れたら、親パッケージを引数にして自メソッドを再帰呼び出しする
                if (this.targetPackageMap.ContainsKey(pac.parentPackageId))
                {
                    PackageVO ppac = this.targetPackageMap[pac.parentPackageId];
                    goBackRetrievePackage(ppac, outList);
                }
                else
                {
                    //                    throw new Exception("親PackageId が不正です : " + pac.parentPackageId);
                    Console.WriteLine("親PackageId が不正です : " + pac.parentPackageId);
                }
            }
            else
            {
                return;
            }

        }


        /// <summary>
        /// Index用データベース(SQLite)に、接続と要素（属性・操作）の情報を登録する
        /// </summary>
        public void doMakeIndex(List<ArtifactVO> artifacts)
        {

            try
            {
                ProjectSetting.load(this.outputDir + "\\project.bdprj");
                IndexDbWriter dbWriter = new IndexDbWriter(this.outputDir, ProjectSetting.getVO().dbName);
                // dbWriter.writeAllConnector(this.allconnectors);
                dbWriter.writeAllElements(artifacts);
                // dbWriter.writeAllBehaviors(this.allArtifacts);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// 要素毎のXMLを elements 配下に出力する
        /// </summary>
        public void doMakeElementFiles(List<ArtifactVO> outArtifactList)
        {
            // outputElementXmlFile メソッドの出力先は ProjectPath 限定になるため、
            // 先に作ったばかりのプロジェクトファイルをロードしておく
            string changedProjectFile = this.outputDir + "\\project.bdprj";

            ProjectSetting.load(changedProjectFile);

            // 
            foreach (ArtifactVO atf in outArtifactList)
            {
                foreach (ElementVO elem in atf.getOwnElements())
                {
                    if( elem.changed != ' ' )
                    {
                        ElementXmlWriter.outputElementXmlFile(elem);
                    }
                }
            }
        }

    }
}
