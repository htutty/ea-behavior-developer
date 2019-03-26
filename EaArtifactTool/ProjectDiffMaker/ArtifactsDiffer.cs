using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;

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
		private ArtifactVO procArtifact = null;
		
		public ArtifactsDiffer() {
		}

		/// <summary>
		/// 比較元、比較先のプロジェクトを引数に取るコンストラクタ
		/// </summary>
		/// <param name="fromProjectFile_">比較元のプロジェクトファイル</param>
		/// <param name="toProjectFile_">比較先のプロジェクトファイル</param>
		public ArtifactsDiffer(string fromProjectFile, string toProjectFile) {
			if ( fromProjectFile != null ) {
				this.fromProjectFile = fromProjectFile;
				this.fromProjectDir = Path.GetDirectoryName(fromProjectFile);
                this.fromArtifactDir = fromProjectDir + "\\artifacts";
            }
			
			if ( toProjectFile != null ) {
				this.toProjectFile = toProjectFile;
				this.toProjectDir = Path.GetDirectoryName(toProjectFile);
                this.toArtifactDir = toProjectDir + "\\artifacts";
            }
			
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
			
			Console.WriteLine("readAllArtifacts(): to");
            ProjectSetting.load(this.toProjectFile);
            this.toArtifacts = readAllArtifacts(this.toArtifactDir);
		}

		/// <summary>
		/// プロジェクト内の全成果物の読み込み
		///  AllArtifacts.xml → atf_xxxx.xml ファイルを読み、全ての成果物パッケージの内容をメモリに読み込む
		/// </summary>
		/// <param name="projectDir"></param>
		/// <returns>全成果物のリスト</returns>
		private List<ArtifactVO> readAllArtifacts( string artifactsDir) {
            ArtifactXmlReader reader = new ArtifactXmlReader(artifactsDir);

            List<ArtifactVO> retList = ArtifactsXmlReader.readArtifactList(artifactsDir, ProjectSetting.getVO().artifactsFile);
			ArtifactXmlReader atfReader = new ArtifactXmlReader(artifactsDir);
			
			foreach( ArtifactVO atf in retList ) {
				// 成果物パッケージ別のXMLファイル読み込み
				atfReader.readArtifactDesc(atf);
			}
			
			// 成果物リストをソートする。 ソートキー＝GUID（自然順序付け）see: BehaviorDevelop.vo.ArtifactVO#compareTo
			retList.Sort();
			return retList;
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		public void mergeAllArtifacts() {
			Int16 lCnt, rCnt;
			ArtifactVO lAtf, rAtf, outAtf;
			List<ArtifactVO> outList = new List<ArtifactVO>();
			for (lCnt=0, rCnt=0; lCnt < fromArtifacts.Count && rCnt < toArtifacts.Count; ) {
				lAtf = fromArtifacts[lCnt];
				rAtf = toArtifacts[rCnt];
				
				// GUID比較で一致した場合: 成果物のパッケージ内の内容比較に移行する
				if (compareArtifactGuid(lAtf, rAtf) == 0) {
					// 現在処理対象の成果物を保持する
					procArtifact = lAtf;
					outAtf = getAgreedContentsOfArtifact(lAtf, rAtf);
					outList.Add(outAtf);
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
			for (lCnt = 0,rCnt = 0; lCnt < leftElements.Count && rCnt < rightElements.Count;) {

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
		
		private static void outputConsole( ArtifactVO atf ) {
			switch( atf.changed ) {
				case 'U':
					Console.WriteLine("一致: GUID="+ atf.guid + ", Name=" + atf.name );
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

            // 差分のdetailファイルが出力されるフォルダを事前に作成する
            makeDetailDirIfNotExist(this.outputDir + "\\detail");

            //BOM無しのUTF8でテキストファイルを作成する
            StreamWriter listsw = new StreamWriter(outputDir + "\\ChangedArtifacts.xml");
			listsw.WriteLine( @"<?xml version=""1.0"" encoding=""utf-8""?> " );
			listsw.WriteLine( "" );
			
			listsw.Write( "<artifacts " );
			listsw.Write( " targetProject='asw' " );
//			listsw.Write( " lastUpdated='" + artifacts.LastUpdated + "' " );
//			listsw.Write( " targetModel='" + artifacts.TargetModel + "' " );
			listsw.WriteLine( " >" );
			
			foreach (ArtifactVO atf in this.outArtifacts) {
				if ( atf.changed != ' ' ) {
                    // このオブジェクト内で現在処理中の成果物を更新する
                    this.procArtifact = atf;

                    outputChangedArtifactList(atf, listsw);
					
					//BOM無しのUTF8でテキストファイルを作成する
					StreamWriter atfsw = new StreamWriter(outputDir + "\\atf_" + atf.guid.Substring(1,36) + ".xml");
					atfsw.WriteLine( @"<?xml version=""1.0"" encoding=""utf-8""?>" );
					atfsw.WriteLine( "" );

					if( atf.package != null ) {
						atfsw.Write( indent(1) + "<artifact " );
						atfsw.Write( " changed='" + atf.changed + "' " );
						atfsw.Write( " guid='" + atf.guid + "' " );
						atfsw.Write( " name='" + escapeXML(atf.name) + "' " );
						atfsw.Write( " path='" + escapeXML(atf.pathName) + "' " );
						atfsw.Write( " project='asw' " );
						atfsw.Write( " stereotype='" + atf.stereoType + "' " );
						atfsw.WriteLine( ">" );
						
						//sw.WriteLine("■成果物(" + atf.changed + "): GUID="+ atf.guid + ", Name=" + atf.name );
						outputChangedPackage(atf.package, 2, atfsw);
						
						atfsw.WriteLine(indent(1) +  "</artifact>" );
					}
					atfsw.Close();
				}
			}

			listsw.WriteLine( "</artifacts>" );
			listsw.Close();
			
			//BOM無しのUTF8でテキストファイルを作成する
			StreamWriter sumsw = new StreamWriter(outputDir + "\\ChangedSummary.csv",
			                                      false, System.Text.Encoding.GetEncoding("shift_jis"));
			sumsw.WriteLine("\"変更内容(CUD)\",\"パッケージGUID\",\"パッケージ名\",\"パッケージパス\",\"変更内容(CUD)\",\"要素GUID\",\"要素名\",\"差分詳細\",\"\"");
			foreach (ArtifactVO atf in this.outArtifacts) {
				if ( atf.changed != ' ' ) {
					outputChangedSummaryArtifact(atf, sumsw);
				}
			}

			sumsw.Close();


			//BOM無しのUTF8でテキストファイルを作成する
			StreamWriter detailsw = new StreamWriter(outputDir + "\\ChangedDetail.csv",
			                                         false, System.Text.Encoding.GetEncoding("shift_jis"));
			detailsw.WriteLine("\"変更内容(CUD)\",\"パッケージGUID\",\"パッケージ名\",\"パッケージパス\",\"変更内容(CUD)\",\"要素GUID\",\"要素名\",\"属性/操作\",\"変更内容(CUD)\",\"属性操作GUID\",\"属性操作名称\",\"左ファイル\",\"右ファイル\"");
			foreach (ArtifactVO atf in this.outArtifacts) {
				if ( atf.changed != ' ' ) {
					outputChangedDetailArtifact(atf, detailsw);
				}
			}

			detailsw.Close();

			StreamWriter prjsw = new StreamWriter(outputDir + "\\project.bdprj");
			prjsw.WriteLine( @"<?xml version=""1.0"" encoding=""utf-8""?> " );
			prjsw.WriteLine( "" );
			
			prjsw.WriteLine( "<project>" );
			prjsw.Write(@" <projectName>merged</projectName>
  <artifactsFile>ChangedArtifacts.xml</artifactsFile>
  <allConnector>AllConnectors.xml</allConnector>
  <dbName>_merged.db</dbName>" );
			prjsw.WriteLine( "</project>" );
			prjsw.Close();

		}


        private static void makeDetailDirIfNotExist(string detailDir)
        {
            // 成果物出力先の artifacts フォルダが存在しない場合
            if (!Directory.Exists(detailDir))
            {
                Directory.CreateDirectory(detailDir);
                Console.WriteLine("出力ディレクトリを作成しました。 : " + detailDir);
            }
        }


        private void outputChangedArtifactList(ArtifactVO atf, StreamWriter sw) {
			sw.Write( "  <artifact " );
			sw.Write( " changed='" + atf.changed + "' " );
			sw.Write( " guid='" + atf.guid + "' " );
			sw.Write( " name='" + escapeXML(atf.name) + "' " );
			sw.Write( " path='" + escapeXML(atf.pathName) + "' " );
			sw.Write( " project='asw' " );
			sw.Write( " stereotype='" + atf.stereoType + "' " );
			sw.WriteLine( "/>" );
		}

		private void outputChangedSummaryArtifact(ArtifactVO atf, StreamWriter sw) {
			outputChangedSummaryPackage(atf, atf.package, sw);
		}

		private void outputChangedSummaryPackage(ArtifactVO atf, PackageVO pkg, StreamWriter sw) {
			foreach( ElementVO e in pkg.elements ) {
				outputChangedSummaryElement(atf, e, sw);
			}

			foreach( PackageVO p in pkg.childPackageList ) {
				outputChangedSummaryPackage(atf, p, sw);
			}
		}
		
		private void outputChangedSummaryElement(ArtifactVO atf, ElementVO elem, StreamWriter sw) {
			string chdesc;
			
			chdesc = "";
			chdesc = chdesc + "attribute:" + elem.attributes.Count + "/";
			chdesc = chdesc + "method:" + elem.methods.Count + "/";
            chdesc = chdesc + "taggedvalue:" + elem.taggedValues.Count;

            sw.Write("\"" + atf.changed + "\"");
			sw.Write(",\"" + atf.guid + "\"");
			sw.Write(",\"" + atf.name + "\"");
			sw.Write(",\"" + atf.pathName + "\"");
			sw.Write(",\"" + elem.changed + "\"");
			sw.Write(",\"" + elem.guid + "\"");
			sw.Write(",\"" + elem.name + "\"");
			sw.Write(",\"" + chdesc + "\"");
			sw.WriteLine("");
		}

		private void outputChangedDetailArtifact(ArtifactVO atf, StreamWriter sw) {
			outputChangedDetailPackage(atf, atf.package, sw);
		}

		private void outputChangedDetailPackage(ArtifactVO atf, PackageVO pkg, StreamWriter sw) {
			foreach( ElementVO e in pkg.elements ) {
				outputChangedDetailElement(atf, e, sw);
			}

			foreach( PackageVO p in pkg.childPackageList ) {
				outputChangedDetailPackage(atf, p, sw);
			}
		}
		
		private void outputChangedDetailElement(ArtifactVO atf, ElementVO elem, StreamWriter sw) {
			foreach ( AttributeVO a in elem.attributes ) {
				outputChangedDetailAttribute(atf, elem, a, sw);
			}

			foreach ( MethodVO m in elem.methods ) {
				outputChangedDetailMethod(atf, elem, m, sw);
			}

            foreach (TaggedValueVO tv in elem.taggedValues)
            {
                outputChangedDetailTaggedValue(atf, elem, tv, sw);
            }
        }


        private void outputChangedDetailTaggedValue(ArtifactVO atf, ElementVO elem, TaggedValueVO tv, StreamWriter sw)
        {
            sw.Write("\"" + atf.changed + "\"");
            sw.Write(",\"" + atf.guid + "\"");
            sw.Write(",\"" + atf.name + "\"");
            sw.Write(",\"" + atf.pathName + "\"");
            sw.Write(",\"" + elem.changed + "\"");
            sw.Write(",\"" + elem.guid + "\"");
            sw.Write(",\"" + elem.name + "\"");
            sw.Write(",\"タグ\"");
            sw.Write(",\"" + tv.changed + "\"");
            sw.Write(",\"" + tv.guid + "\"");
            sw.Write(",\"" + tv.name + "\"");

            if (tv.changed == 'D' || tv.changed == 'U')
            {
                sw.Write(",\"#taggedvalue_" + tv.guid.Substring(1, 36) + "_L.xml\"");
            }
            else
            {
                sw.Write(",\"\"");
            }

            if (tv.changed == 'C' || tv.changed == 'U')
            {
                sw.Write(",\"#taggedvalue_" + tv.guid.Substring(1, 36) + "_R.xml\"");
            }
            else
            {
                sw.Write(",\"\"");
            }

            sw.WriteLine("");
        }


        private void outputChangedDetailAttribute( ArtifactVO atf, ElementVO elem, AttributeVO attr, StreamWriter sw ) {
			sw.Write("\"" + atf.changed + "\"");
			sw.Write(",\"" + atf.guid + "\"");
			sw.Write(",\"" + atf.name + "\"");
			sw.Write(",\"" + atf.pathName + "\"");
			sw.Write(",\"" + elem.changed + "\"");
			sw.Write(",\"" + elem.guid + "\"");
			sw.Write(",\"" + elem.name + "\"");
			sw.Write(",\"属性\"");
			sw.Write(",\"" + attr.changed + "\"");
			sw.Write(",\"" + attr.guid + "\"");
			sw.Write(",\"" + attr.name + "\"");
			
			if( attr.changed == 'D' || attr.changed == 'U' ) {
				sw.Write(",\"#attribute_" + attr.guid.Substring(1,36) + "_L.xml\"");
			} else {
				sw.Write(",\"\"");
			}

			if( attr.changed == 'C' || attr.changed == 'U' ) {
				sw.Write(",\"#attribute_" + attr.guid.Substring(1,36) + "_R.xml\"");
			} else {
				sw.Write(",\"\"");
			}

			sw.WriteLine("");
		}
		
		private void outputChangedDetailMethod( ArtifactVO atf, ElementVO elem, MethodVO mth, StreamWriter sw ) {
			sw.Write("\"" + atf.changed + "\"");
			sw.Write(",\"" + atf.guid + "\"");
			sw.Write(",\"" + atf.name + "\"");
			sw.Write(",\"" + atf.pathName + "\"");
			sw.Write(",\"" + elem.changed + "\"");
			sw.Write(",\"" + elem.guid + "\"");
			sw.Write(",\"" + elem.name + "\"");
			sw.Write(",\"操作\"");
			sw.Write(",\"" + mth.changed + "\"");
			sw.Write(",\"" + mth.guid + "\"");
			sw.Write(",\"" + mth.name + "\"");

			if( mth.changed == 'D' || mth.changed == 'U' ) {
				sw.Write(",\"#method_" + mth.guid.Substring(1,36) + "_L.xml\"");
			} else {
				sw.Write(",\"\"");
			}

			if( mth.changed == 'C' || mth.changed == 'U' ) {
				sw.Write(",\"#method_" + mth.guid.Substring(1,36) + "_R.xml\"");
			} else {
				sw.Write(",\"\"");
			}

			sw.WriteLine("");
		}

		
		private void outputChangedPackage(PackageVO pkg, int depth, StreamWriter sw) {

			sw.Write(indent(depth) + "<package ");
			sw.Write(" guid='" + pkg.guid + "' ");
			sw.Write(" name='" + escapeXML(pkg.name) + "' ");
			sw.Write(" isControlled='" + pkg.isControlled + "' ");
			sw.Write(" changed='" + pkg.changed + "' ");
			sw.WriteLine(" >");
			
			if (pkg.childPackageList.Count > 0) {
				foreach(PackageVO p in pkg.childPackageList) {
					outputChangedPackage(p, depth+1, sw);
				}
			}
			
			if (pkg.elements.Count > 0 ) {
				outputElements(pkg.elements, depth+1, sw);
			}
			
			sw.WriteLine(indent(depth) + "</package>");
		}

		
		private void outputElements(List<ElementVO> elements, int depth, StreamWriter sw) {
			foreach( ElementVO elem in elements ) {
				outputClass(elem, depth, sw);
			}
		}

		private void outputClass(ElementVO elemvo, int depth, StreamWriter sw) {
			sw.Write(indent(depth) + "<element ");
			sw.Write(" changed='" + elemvo.changed + "' ");
			sw.Write(" guid='" + escapeXML(elemvo.guid) + "' ");
			sw.Write(" tpos='" + elemvo.treePos + "' ");
			sw.Write(" type='" + elemvo.objectType + "' ");
			sw.Write(" name='" + escapeXML(elemvo.name) + "' ");
			sw.Write(" alias='" + escapeXML(elemvo.alias) + "' ");
			
			if ( elemvo.stereoType != null ) {
				sw.Write( " stereotype='" + elemvo.stereoType + "' " );
			}
			sw.WriteLine(">");
			
			if (elemvo.notes != null) {
				sw.WriteLine(indent(depth) +"  <notes>" + escapeXML(elemvo.notes) + "</notes>" );
			}
			
			// 1.2.1 クラスのタグ付き値出力
			if (elemvo.taggedValues.Count > 0) {
				outputClassTags(elemvo, depth+1, sw);
			}
			
			// 1.2.2 クラスの属性出力
			if (elemvo.attributes.Count > 0) {
				outputClassAttributes(elemvo, depth+1, sw);
			}
			
			// 1.2.3 クラスの操作（メソッド）出力
			if (elemvo.methods.Count > 0) {
				outputClassMethods(elemvo, depth+1, sw);
			}
			
			sw.WriteLine(indent(depth) + "</element>");
		}


        // 1.2.1 クラスのタグ付き値を出力
        private void outputClassTags(ElementVO currentElement, int depth, StreamWriter sw)
        {

            if (currentElement.taggedValues.Count <= 0)
            {
                return;
            }

            sw.Write(indent(depth) + "<taggedValues>");

            // 　取得できたタグ付き値の情報をファイルに展開する
            foreach (TaggedValueVO tagv in currentElement.taggedValues ) {
                // タグ付き値(tvノード)の出力
                outputTaggedValue(tagv, depth + 1, sw);

                switch (tagv.changed)
                {
                    case 'U':
                        outputOriginalTaggedValueFile(tagv, "L");
                        outputOriginalTaggedValueFile(tagv, "R");
                        break;

                    case 'C':
                        outputOriginalTaggedValueFile(tagv, "R");
                        break;
                    case 'D':
                        outputOriginalTaggedValueFile(tagv, "L");
                        break;
                }

            }

            sw.WriteLine(indent(depth) + "</taggedValues>");
        }

        private void outputTaggedValue(TaggedValueVO tagv, int depth, StreamWriter sw)
        {
            sw.WriteLine(indent(depth) + "<tv changed='" + tagv.changed + "' guid='" + escapeXML(tagv.guid) + "' name='" + escapeXML(tagv.name) + "' value='" + escapeXML(tagv.tagValue) + "'>");

            if ("<memo>".Equals(tagv.tagValue))
            {
                if (tagv.notes != null)
                {
                    sw.WriteLine(indent(depth) + "  <notes>" + escapeXML(tagv.notes) + "</notes>");
                }
            }

            // 変更区分="U" かつ src, dest のタグ付き値が設定済みの場合
            if (tagv.changed == 'U' && tagv.srcTaggedValue != null && tagv.destTaggedValue != null)
            {
                sw.WriteLine(indent(depth) + "  <srcTaggedValue>");
                outputDiffedTaggedValue(tagv.srcTaggedValue, depth + 2, sw);
                sw.WriteLine(indent(depth) + "  </srcTaggedValue>" );

                sw.WriteLine(indent(depth) + "  <destTaggedValue>");
                outputDiffedTaggedValue(tagv.destTaggedValue, depth + 2, sw);
                sw.WriteLine(indent(depth) + "  </destTaggedValue>");
            }



            sw.WriteLine(indent(depth) + "</tv>");

        }

        private void outputDiffedTaggedValue(TaggedValueVO tagv, int depth, StreamWriter sw)
        {
            sw.WriteLine(indent(depth) + "<tv guid='" + escapeXML(tagv.guid) + "' name='" + escapeXML(tagv.name) + "' value='" + escapeXML(tagv.tagValue) + "'>");

            if ("<memo>".Equals(tagv.tagValue))
            {
                if (tagv.notes != null)
                {
                    sw.WriteLine(indent(depth) + "  <notes>" + escapeXML(tagv.notes) + "</notes>");
                }
            }

            sw.WriteLine(indent(depth) + "</tv>");
        }


        // 1.2.2 クラスの属性を出力
        private void outputClassAttributes( ElementVO currentElement, int depth, StreamWriter sw ) {
			
			if ( currentElement.attributes.Count <= 0 ) {
				return;
			}
			
			//　取得できた属性の情報をファイルに展開する
			foreach(AttributeVO attr in currentElement.attributes) {
				outputAttribute(attr, depth, sw);
				
				switch( attr.changed ) {
					case 'U': 
						outputOriginalAttributeFile(attr,"L");
						outputOriginalAttributeFile(attr,"R");
						break;

					case 'C': 
						outputOriginalAttributeFile(attr,"R");
						break;
					case 'D': 
						outputOriginalAttributeFile(attr,"L");
						break;
				}
			}
			
		}

		private void outputAttribute(AttributeVO att, int depth, StreamWriter sw )
        {
			sw.Write( indent(depth) + "<attribute " );
			if( att.changed != ' ' ) {
				sw.Write( " changed='" + att.changed + "' " );
			}
			sw.Write( " guid='" + escapeXML(att.guid) + "' " );
			sw.Write( " pos='" + att.pos + "' " );
//				sw.Write( " type='" + escapeXML(m_Att.EA_Type) + "' " );
			sw.Write( " name='" + escapeXML(att.name) + "' " );
			sw.Write( " alias='" + escapeXML(att.alias) + "' " );
			sw.Write( " position='" + att.pos + "'" );
			
			if (att.stereoType != null) {
				sw.Write( " stereotype='" + att.stereoType + "' " );
			}
			
			sw.WriteLine( " >" );
			
//				if (m_Att.Default != null) {
//					sw.WriteLine( indent(depth) + "  <default>" + escapeXML(m_Att.Default) + "</default>" );
//				}
//				sw.WriteLine( indent(depth) + "  <visibility>" + m_Att.Visibility + "</visibility>"  );
			
			if (att.notes != null) {
				sw.WriteLine( indent(depth) + "  <notes>" + escapeXML(att.notes) + "</notes>" );
			}

            if (att.changed == 'U' && att.srcAttribute != null && att.destAttribute != null)
            {
                sw.WriteLine(indent(depth + 1) + "<srcAttribute>");
                outputAttribute(att.srcAttribute, depth + 2, sw);
                sw.WriteLine(indent(depth + 1) + "</srcAttribute>");

                sw.WriteLine(indent(depth + 1) + "<destAttribute>");
                outputAttribute(att.destAttribute, depth + 2, sw);
                sw.WriteLine(indent(depth + 1) + "</destAttribute>");
            }

			sw.WriteLine( indent(depth) + "</attribute>" );
		}
		
		

		// 1.2.3 クラスの操作を出力
		private void outputClassMethods(ElementVO currentElement, int depth, StreamWriter sw) {
			
			if(currentElement.methods.Count <= 0 ) {
				return ;
			}
			
			//　取得できた操作の情報をファイルに展開する
			foreach( MethodVO mth in currentElement.methods ) {
				outputMethod(mth, depth, sw);
				
				switch( mth.changed ) {
					case 'U': 
						outputOriginalMethodFile(mth,"L");
						outputOriginalMethodFile(mth,"R");
						break;
					case 'C': 
						outputOriginalMethodFile(mth,"R");
						break;
					case 'D': 
						outputOriginalMethodFile(mth,"L");
						break;
				}
			}
		}

		
		private void outputMethod(MethodVO mth, int depth, StreamWriter sw) {
			sw.Write( indent(depth) + "<method " );

			if( mth.changed != ' ' ) {
				sw.Write( " changed='" + mth.changed + "' " );
			}
			sw.Write( " guid='" + escapeXML(mth.guid) + "' " );
			sw.Write( " pos='" + mth.pos + "' " );
			sw.Write( " name='" + escapeXML(mth.name) + "' " );
			sw.Write( " alias='" + escapeXML(mth.alias) + "' " );
			if(mth.stereoType != null) {
				sw.Write( " stereotype='" + mth.stereoType + "' " );
			}
			sw.WriteLine( ">" );
			
			sw.WriteLine( indent(depth) + "  <visibility>" + mth.visibility + "</visibility>"  );
			sw.WriteLine( indent(depth) + "  <returnType>" + escapeXML(mth.returnType) + "</returnType>"  );
			
			// 1.2.3.1.1 メソッドパラメータの出力
			outputMethodParams( mth, depth, sw );
			
			if (mth.notes != null) {
				sw.WriteLine( indent(depth) + "  <notes>" + escapeXML( mth.notes ) + "</notes>" );
			}
			
			if (mth.behavior != null) {
				sw.WriteLine( indent(depth) + "  <behavior>" + escapeXML( mth.behavior ) + "</behavior>" );
			}

            // 1.2.3.1.2 メソッドのタグ付き値出力
            // Call outputMethodTags( mth, resp );

            if (mth.changed == 'U' && mth.srcMethod != null && mth.destMethod != null)
            {
                sw.WriteLine(indent(depth + 1) + "<srcMethod>");
                outputMethod(mth.srcMethod, depth + 2, sw);
                sw.WriteLine(indent(depth + 1) + "</srcMethod>");

                sw.WriteLine(indent(depth + 1) + "<destMethod>");
                outputMethod(mth.destMethod, depth + 2, sw);
                sw.WriteLine(indent(depth + 1) + "</destMethod>");
            }

            sw.WriteLine( indent(depth) + "</method>" );
		}

		//  1.2.3.1.1 メソッドのパラメータ出力
		private void outputMethodParams(MethodVO mth, int depth, StreamWriter sw) {

			if (mth.parameters.Count <= 0) {
				return ;
			}
			
			// メソッドのパラメータ
			foreach( ParameterVO prm in mth.parameters ) {
				sw.Write( indent(depth) + "  <parameter " );
				sw.Write( "guid='" + escapeXML(prm.guid) + "' " );
				sw.Write( "name='" + escapeXML(prm.name) + "' " );
				sw.Write( "type='" + escapeXML(prm.eaType) + "' " );
				sw.Write( "alias='" + escapeXML(prm.alias) + "' " );
				sw.Write( "stereotype='" + escapeXML(prm.stereoType) + "' " );
				sw.Write( "pos='" + prm.pos + "' " );
				sw.Write( "classifier='" + prm.classifierID + "' " );
				sw.Write( "default='" + escapeXML(prm.defaultValue) + "' " );
				sw.Write( "const='" + prm.isConst + "' " );
				sw.Write( "kind='" + prm.kind + "' " );
				sw.Write( "objectType='" + prm.objectType + "' " );
				sw.Write( "style='" + prm.styleEx + "' " );
				
				sw.Write( ">" );

				
				// <parameter guid='{ACBC066F-A1D6-45c2-B271-F7A32841B467}' name='社員情報検索サービス入力用DTO' type='社員情報検索サービス入力用DTO'
				// alias='' stereotype='' pos='0' classifier='0' default='' const='False' kind='in' objectType='25' style='社員情報検索サービス入力用DTO' >

				if (prm.notes != null) {
					sw.WriteLine( indent(depth) + "  <notes>" + escapeXML( prm.notes ) + "</notes>");
				}

				sw.WriteLine( indent(depth) + "  </parameter>" );
			}
			
		}
		
		
		private string indent(int depth) {
			string retStr = "";
			Int32  i;
			
			for(i=0; i < depth; i++) {
				retStr = retStr + "  ";
			}
			return retStr;
		}
		
		private string escapeXML( string orig ) {
			if (orig == null) {
				return "";
			}
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder(orig);
			sb.Replace("&", "&amp;");
			sb.Replace("<", "&lt;");
			sb.Replace(">", "&gt;");
			sb.Replace("\"", "&quot;");
			sb.Replace("'", "&apos;");
			return sb.ToString();
		}



		/// <summary>
		/// 比較して差異があった属性に対して左右それぞれのオリジナル情報を出力し、ピンポイントの比較ができるようにする
		/// </summary>
		/// <param name="attr">該当属性</param>
		/// <param name="leftRight"> "L" もしくは "R" を指定する（出力ファイル名のサフィックスになる）</param>
		private void outputOriginalAttributeFile(AttributeVO attr, string leftRight) {

			// 明細ファイル出力フラグが立っていなければそのままリターン
			if( !outputDetailFileFlg ) {
				return ;
			}

			XmlDocument xmlDocument = new XmlDocument();
			//	' XML宣言部分生成
			// 	Set head = xmlDocument.createProcessingInstruction("xml", "version='1.0'")
			XmlProcessingInstruction head = xmlDocument.CreateProcessingInstruction("xml", "version='1.0'");

			//' XML宣言部分設定
			xmlDocument.AppendChild(head);

			string xmlDir = null;
			if ( "L".Equals(leftRight) ) {
				xmlDir = this.fromArtifactDir;
			} else {
				xmlDir = this.toArtifactDir;
			}
			ArtifactXmlReader atfReader = new ArtifactXmlReader(xmlDir);
			
			// 現在処理中の成果物のGUID、および属性のGUIDから（左もしくは右の）XMLノードを取得
			XmlNode attrNode = atfReader.readAttributeNode(procArtifact.guid, attr.guid);
			
            // 成果物のGUID,属性のGUIDから取得した属性NodeがNullなら出力をスキップする
            if ( attrNode != null )
            {
			    // 新しいrootノードを、成果物XMLからインポートする形で作成
			    XmlNode root = xmlDocument.ImportNode( attrNode, true );
			
			    // ルート配下に引数のドキュメントを追加
			    xmlDocument.AppendChild(root);

			    // この内容で操作の詳細に記録する
			    xmlDocument.Save(outputDir + "\\detail\\#attribute_" + attr.guid.Substring(1,36) + "_" + leftRight + ".xml");
            }

        }


        /// <summary>
        /// 比較して差異があったメソッドに対して左右それぞれのオリジナル情報を出力し、ピンポイントの比較ができるようにする
        /// </summary>
        /// <param name="mth">該当メソッド</param>
        /// <param name="leftRight"> "L" もしくは "R" を指定する（出力ファイル名のサフィックスになる）</param>
        private void outputOriginalMethodFile(MethodVO mth, string leftRight) {
			// 明細ファイル出力フラグが立っていなければそのままリターン
			if( !outputDetailFileFlg ) {
				return ;
			}
		
			XmlDocument xmlDocument = new XmlDocument();
			
			//	' XML宣言部分生成
			// 	Set head = xmlDocument.createProcessingInstruction("xml", "version='1.0'")
//			XmlProcessingInstruction head = xmlDocument.CreateProcessingInstruction("xml", "version='1.0'");

			//' XML宣言部分設定
			// xmlDocument.AppendChild(head);

			string xmlDir = null;
			if ( "L".Equals(leftRight) ) {
				xmlDir = this.fromArtifactDir;
			} else {
				xmlDir = this.toArtifactDir;
			}
			ArtifactXmlReader atfReader = new ArtifactXmlReader(xmlDir);
			
			// 現在処理中の成果物のGUID、およびメソッドのGUIDから（左もしくは右の）XMLノードを取得
			XmlNode methodNode = atfReader.readMethodNode(procArtifact.guid, mth.guid );

            // 成果物のGUID, メソッドのGUIDから読み取ったメソッドNodeがNULLだったらスキップ
            if ( methodNode != null ) { 
			    // 新しいrootノードを、成果物XMLからインポートする形で作成
			    XmlNode root = xmlDocument.ImportNode( methodNode, true );
			
			    // ルート配下に引数のドキュメントを追加
			    xmlDocument.AppendChild(root);

			    // この内容で操作の詳細に記録する
			    xmlDocument.Save( this.outputDir + "\\detail\\#method_" + mth.guid.Substring(1,36) + "_" + leftRight + ".xml" );
            }
        }


        /// <summary>
        /// 比較して差異があったメソッドに対して左右それぞれのオリジナル情報を出力し、ピンポイントの比較ができるようにする
        /// </summary>
        /// <param name="tagVal">該当メソッド</param>
        /// <param name="leftRight"> "L" もしくは "R" を指定する（出力ファイル名のサフィックスになる）</param>
        private void outputOriginalTaggedValueFile(TaggedValueVO tagVal, string leftRight)
        {
            // 明細ファイル出力フラグが立っていなければそのままリターン
            if (!outputDetailFileFlg)
            {
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();

            string xmlDir = null;
            if ("L".Equals(leftRight))
            {
                xmlDir = this.fromArtifactDir;
            }
            else
            {
                xmlDir = this.toArtifactDir;
            }
            ArtifactXmlReader atfReader = new ArtifactXmlReader(xmlDir);

            // 現在処理中の成果物のGUID、およびメソッドのGUIDから（左もしくは右の）XMLノードを取得
            XmlNode taggedValueNode = atfReader.readTaggedValueNode(procArtifact.guid, tagVal.guid);

            // 成果物のGUID, メソッドのGUIDから読み取ったメソッドNodeがNULLだったらスキップ
            if (taggedValueNode != null)
            {
                // 新しいrootノードを、成果物XMLからインポートする形で作成
                XmlNode root = xmlDocument.ImportNode(taggedValueNode, true);

                // ルート配下に引数のドキュメントを追加
                xmlDocument.AppendChild(root);

                // この内容で操作の詳細に記録する
                xmlDocument.Save(this.outputDir + "\\detail\\#taggedValue_" + tagVal.guid.Substring(1, 36) + "_" + leftRight + ".xml");
            }
        }

    }
}
