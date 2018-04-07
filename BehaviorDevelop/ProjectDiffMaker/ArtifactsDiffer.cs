/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/31
 * Time: 9:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;

using BehaviorDevelop.util;
using BehaviorDevelop.vo;

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
		
		private string fromProjectFile = null;
		private string fromProjectDir = null;
		private string toProjectFile = null;
		private string toProjectDir = null;
		
		public string outputDir;
		
		private List<ArtifactVO> fromArtifacts;
		private List<ArtifactVO> toArtifacts;
		private List<ArtifactVO> outArtifacts;
		
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
			}
			
			if ( toProjectFile != null ) {
				this.toProjectFile = toProjectFile;
				this.toProjectDir = Path.GetDirectoryName(toProjectFile);
			}
			
			Console.WriteLine("fromdir = " + this.fromProjectDir);
			Console.WriteLine("todir = " + this.toProjectDir);
		}
		

		/// <summary>
		/// 比較元、比較先プロジェクトの全成果物リストを読み込み
		/// </summary>
		public void readBothArtifacts() {
			Console.WriteLine("readAllArtifacts(): from");
			this.fromArtifacts = readAllArtifacts(this.fromProjectDir);
			
			Console.WriteLine("readAllArtifacts(): to");
			this.toArtifacts = readAllArtifacts(this.toProjectDir);			
		}

		/// <summary>
		/// プロジェクト内の全成果物の読み込み
		///  ALL_Artifacts.xml → atf_xxxx.xml ファイルを読み、全ての成果物パッケージの内容をメモリに読み込む
		/// </summary>
		/// <param name="projectDir_"></param>
		/// <returns>全成果物のリスト</returns>
		private List<ArtifactVO> readAllArtifacts( string projectDir_ ) {
			List<ArtifactVO> retList = ArtifactsXmlReader.readArtifactList(projectDir_);
			ArtifactXmlReader atfReader = new ArtifactXmlReader(projectDir_);
			
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
					outAtf = getAgreedContentsOfArtifact(lAtf, rAtf);
					outList.Add(outAtf);
					lCnt++;
					rCnt++;
				} else {
					// GUID比較で一致しなかった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
					if(compareArtifactGuid(lAtf, rAtf) > 0) {
						outAtf = rAtf;
						outAtf.changed = 'C';
						
						setChangedToChilds(outAtf.package, 'C');
						outList.Add(outAtf);
						rCnt++;
					} else {
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

			
			// パッケージが保持する要素リストの比較
			ElementVO lElm, rElm, oElm;
			List<ElementVO> outElements = new List<ElementVO>();
			for (lCnt=0, rCnt=0; lCnt < leftPkg.elements.Count && rCnt < rightPkg.elements.Count; ) {
				// 左側が最終の要素に達した場合
				if( lCnt >= leftPkg.elements.Count ) {
					// 右側の要素が追加されたものとして出力パッケージリストに追加
					oElm = rightPkg.elements[rCnt];
					oElm.changed = 'C';
					outElements.Add(oElm);
					rCnt++;
					continue;
				} 

				// 右側が最終の要素に達した場合
				if( rCnt >= rightPkg.elements.Count ) {
					// 左側の要素が削除されたものとして出力パッケージリストに追加
					oElm = leftPkg.elements[lCnt];
					oElm.changed = 'D';
					outElements.Add(oElm);
					lCnt++;
					continue;
				} 

				lElm = leftPkg.elements[lCnt];
				rElm = rightPkg.elements[rCnt];
				
				// GUIDで一致するものがあった場合: 
				if (compareElementGuid(lElm, rElm) == 0) {
					oElm = getDisagreedElement(lElm, rElm);
					if (oElm != null) {
						oElm.changed = 'U';
						outElements.Add(oElm);
					}
					
					lCnt++;
					rCnt++;
				} else {
					// GUIDで一致するものが無かった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
					if(compareElementGuid(lElm, rElm) > 0) {
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

		private void setChangedToChilds(PackageVO pkg, char updChanged) {
			pkg.changed = updChanged;
			foreach( PackageVO p in pkg.childPackageList ) {
				setChangedToChilds(p, updChanged);
			}
			
			foreach( ElementVO e in pkg.elements ) {
				e.changed = updChanged;
			}

			return;			
		}

		
		/// <summary>
		/// マージ時に使用：GUIDによる成果物の比較
		/// </summary>
		/// <param name="leftAtf">左の成果物</param>
		/// <param name="rightAtf">右の成果物</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private int compareArtifactGuid(ArtifactVO leftAtf, ArtifactVO rightAtf) {
			return leftAtf.guid.CompareTo(rightAtf.guid);
		}
		
		/// <summary>
		/// マージ時に使用：GUIDによるパッケージの比較
		/// </summary>
		/// <param name="leftAtf">左のパッケージ</param>
		/// <param name="rightAtf">右のパッケージ</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private int comparePackageGuid(PackageVO leftPkg, PackageVO rightPkg) {
			return leftPkg.guid.CompareTo(rightPkg.guid);
		}

		/// <summary>
		/// マージ時に使用：GUIDによる要素の比較
		/// </summary>
		/// <param name="leftAtf">左の要素</param>
		/// <param name="rightAtf">右の要素</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private int compareElementGuid(ElementVO leftElm, ElementVO rightElm) {
			return leftElm.guid.CompareTo(rightElm.guid);
		}

		/// <summary>
		/// マージ時に使用：GUIDによる属性の比較
		/// </summary>
		/// <param name="leftAtr">左の属性</param>
		/// <param name="rightAtr">右の属性</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private int compareAttributeGuid(AttributeVO leftAtr, AttributeVO rightAtr) {
			return leftAtr.guid.CompareTo(rightAtr.guid);
		}

		/// <summary>
		/// マージ時に使用：GUIDによる操作の比較
		/// </summary>
		/// <param name="leftAtr">左の操作</param>
		/// <param name="rightAtr">右の操作</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private int compareMethodGuid(MethodVO leftMth, MethodVO rightMth) {
			return leftMth.guid.CompareTo(rightMth.guid);
		}		
		
		/// <summary>
		/// 要素の不一致部分の抽出
		/// </summary>
		/// <param name="leftElem">左の要素</param>
		/// <param name="rightElem">右の要素</param>
		/// <returns></returns>
		private ElementVO getDisagreedElement(ElementVO leftElm, ElementVO rightElm) {

			// 要素自体が保持するプロパティの比較
			ElementVO outElm;
			if( !leftElm.name.Equals(rightElm.name) ) {
				outElm = rightElm;
				outElm.changed = 'U';
			} else {
				outElm = leftElm;
				outElm.changed = ' ';
			}
			
			if( !skipElementTPosFlg ) {
				if( leftElm.treePos != rightElm.treePos ) {
					outElm = rightElm;
					outElm.changed = 'U';
				} else {
					outElm = leftElm;
					outElm.changed = ' ';
				}
			}
			

			Int16 lCnt, rCnt;

			// 要素が保持する属性リストの比較
			AttributeVO lAtr, rAtr, oAtr;
			List<AttributeVO> outAttributeList = new List<AttributeVO>();
			for (lCnt=0, rCnt=0; lCnt < leftElm.attributes.Count && rCnt < rightElm.attributes.Count; ) {
				// 左側が最終の属性に達した場合
				if( lCnt >= leftElm.attributes.Count ) {
					// 右側の属性が追加されたものとして出力パッケージリストに追加
					oAtr = rightElm.attributes[rCnt];
					oAtr.changed = 'C';
					outAttributeList.Add(oAtr);
					rCnt++;
					continue;
				} 

				// 右側が最終の属性に達した場合
				if( rCnt >= rightElm.attributes.Count ) {
					// 左側の属性が削除されたものとして出力パッケージリストに追加
					oAtr = leftElm.attributes[lCnt];
					oAtr.changed = 'D';
					outAttributeList.Add(oAtr);
					lCnt++;
					continue;
				} 
				
				lAtr = leftElm.attributes[lCnt];
				rAtr = rightElm.attributes[rCnt];
				
				// GUIDの比較で一致した場合
				if (compareAttributeGuid(lAtr, rAtr) == 0) {
					oAtr = getDisagreedAttribute(lAtr, rAtr);
					if (oAtr != null) {
						oAtr.changed = 'U';
						outAttributeList.Add(oAtr);
					}
					lCnt++;
					rCnt++;
				} else {
					// 比較で一致するものが無かった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
					if(compareAttributeGuid(lAtr, rAtr) > 0) {
						oAtr = rAtr;
						oAtr.changed = 'C';
						outAttributeList.Add(oAtr);
						outputOriginalAttributeFile(rAtr,"R");

						rCnt++;
					} else {
						oAtr = lAtr;
						oAtr.changed = 'D';
						outAttributeList.Add(oAtr);
						outputOriginalAttributeFile(lAtr,"L");

						lCnt++;
					}
				}
			}
			outElm.attributes = outAttributeList;

			
			// 要素が保持するメソッドリストの比較			
			MethodVO lMth, rMth, oMth;
			List<MethodVO> outMethods = new List<MethodVO>();
			for (lCnt=0, rCnt=0; lCnt < leftElm.methods.Count && rCnt < rightElm.methods.Count; ) {
				// 左側が最終の操作に達した場合
				if( lCnt >= leftElm.methods.Count ) {
					// 右側の操作が追加されたものとして出力パッケージリストに追加
					oMth = rightElm.methods[rCnt];
					oMth.changed = 'C';
					outMethods.Add(oMth);
					rCnt++;
					continue;
				}

				// 右側が最終の操作に達した場合
				if( rCnt >= rightElm.methods.Count ) {
					// 左側の操作が削除されたものとして出力パッケージリストに追加
					oMth = leftElm.methods[lCnt];
					oMth.changed = 'D';
					outMethods.Add(oMth);
					lCnt++;
					continue;
				} 
				
				
				lMth = leftElm.methods[lCnt];
				rMth = rightElm.methods[rCnt];
				
				// GUIDで一致するものがあった場合: 
				if (compareMethodGuid(lMth, rMth) == 0) {
					oMth = getDisagreedMethod(lMth, rMth);
					if (oMth != null) {
						oMth.changed = 'U';
						outMethods.Add(oMth);
					}
					
					lCnt++;
					rCnt++;
				} else {
					// GUIDで一致するものが無かった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
					if(compareMethodGuid(lMth, rMth) > 0) {
						rMth.changed = 'C';
						outputOriginalMethodFile(rMth,"R");

						outMethods.Add(rMth);
						rCnt++;
					} else {
						lMth.changed = 'D';
						outMethods.Add(lMth);
						outputOriginalMethodFile(lMth,"L");

						lCnt++;
					}
				}
			}
			outElm.methods = outMethods;

			
			if (outElm.attributes.Count == 0 && outElm.methods.Count == 0) {
			    if( outElm.changed == ' ') {
					return null;
				} else {
					outElm.changed = 'U';
					return outElm;
				}
			} else {
				outElm.changed = 'U';
				return outElm;
			}

		}
		
		/// <summary>
		/// 不一致な属性の抽出
		/// </summary>
		/// <param name="leftAtr"></param>
		/// <param name="rightAtr"></param>
		/// <returns></returns>
		private AttributeVO getDisagreedAttribute(AttributeVO leftAtr, AttributeVO rightAtr) {
			AttributeVO outAtr;
			
			outAtr = leftAtr.Clone();
			outAtr.changed = ' ';
			
			if( !compareNullable(leftAtr.name, rightAtr.name) ) {
				outAtr.name = leftAtr.name + " → " + rightAtr.name;
				outAtr.changed = 'U';
			} 
			
			if( !compareNullable(leftAtr.stereoType, rightAtr.stereoType) ) {
				outAtr.stereoType = leftAtr.stereoType + " → " + rightAtr.stereoType;
				outAtr.changed = 'U';
			}
			
			if( !compareNullable(leftAtr.alias, rightAtr.alias) ) {
				outAtr.alias = leftAtr.alias + " → " + rightAtr.alias;
				outAtr.changed = 'U';
			} 

			if( !skipAttributePosFlg ) {
				if( leftAtr.pos != rightAtr.pos ) {
					outAtr.pos = rightAtr.pos;
					outAtr.changed = 'U';
				} 
			}
			
			if( !skipAttributeNoteFlg ) {
				if( !compareNullable(leftAtr.notes, rightAtr.notes) ) {
					outAtr.notes = leftAtr.notes + "\r\n------ ↓ ↓ ↓ ↓ ------\r\n" + rightAtr.notes;
					outAtr.changed = 'U';
				} 
			}
			
			
			if ( outAtr.changed == ' ' ) {
				return null;
			} else {
				outputOriginalAttributeFile(leftAtr,"L");
				outputOriginalAttributeFile(rightAtr,"R");
				
				return outAtr ;
			}
			
		}
		
		
		/// <summary>
		/// 不一致な操作の抽出
		/// </summary>
		/// <param name="leftMth"></param>
		/// <param name="rightMth"></param>
		/// <returns></returns>
		private MethodVO getDisagreedMethod(MethodVO leftMth, MethodVO rightMth) {
			MethodVO outMth;
			
			outMth = leftMth.Clone();
			outMth.changed = ' ';
			
			if( !compareNullable(leftMth.name, rightMth.name) ) {
				outMth.name = leftMth.name + " → " + rightMth.name;
				outMth.changed = 'U';
			} 
			
			if( !compareNullable(leftMth.stereoType, rightMth.stereoType) ) {
				outMth.stereoType = leftMth.stereoType + " → " + rightMth.stereoType;
				outMth.changed = 'U';
			}
			
			if( !compareNullable(leftMth.alias, rightMth.alias) ) {
				outMth.alias = leftMth.alias + " → " + rightMth.alias;
				outMth.changed = 'U';
			} 

			if( !compareNullable(leftMth.returnType, rightMth.returnType) ) {
				outMth.returnType = leftMth.returnType + " → " + rightMth.returnType;
				outMth.changed = 'U';
			}

			if( !compareNullable(leftMth.visibility, rightMth.visibility) ) {
				outMth.visibility = leftMth.visibility + " → " + rightMth.visibility;
				outMth.changed = 'U';
			}			
			
			if( !skipMethodPosFlg ) {
				if( leftMth.pos != rightMth.pos ) {
					outMth.pos = rightMth.pos;
					outMth.changed = 'U';
				} 
			}

			if( !skipMethodNoteFlg ) {
				if( !compareNullable(leftMth.notes, rightMth.notes) ) {
					outMth.notes = leftMth.notes + "\r\n------ ↓ ↓ ↓ ↓ ------\r\n" + rightMth.notes;
					outMth.changed = 'U';
				}
			}
			
			if( !compareNullable(leftMth.behavior, rightMth.behavior) ) {
				outMth.behavior = leftMth.behavior + "\r\n------ ↓ ↓ ↓ ↓ ------\r\n" + rightMth.behavior;
				outMth.changed = 'U';
			}
			
			if ( outMth.changed == ' ' ) {
				return null;
			} else {
				outputOriginalMethodFile(leftMth,"L");
				outputOriginalMethodFile(rightMth,"R");
				
				return outMth ;
			}
		}

		/// <summary>
		/// 比較して差異があった属性に対して左右それぞれのオリジナル情報を出力し、ピンポイントの比較ができるようにする
		/// </summary>
		/// <param name="attr">該当属性</param>
		/// <param name="leftRight"> "L" もしくは "R" を指定する（出力ファイル名のサフィックスになる）</param>
		private void outputOriginalAttributeFile(AttributeVO attr, string leftRight) {

			//BOM無しのUTF8でテキストファイルを作成する
			StreamWriter attrsw = new StreamWriter(outputDir + "\\detail\\#attribute_" + attr.guid.Substring(1,36) + "_" + leftRight + ".xml");
			attrsw.WriteLine( @"<?xml version=""1.0"" encoding=""utf-8""?> " );
			attrsw.WriteLine( "" );

			outputAttribute(attr, 0, attrsw);

			attrsw.Close();

		}
		
		/// <summary>
		/// 比較して差異があったメソッドに対して左右それぞれのオリジナル情報を出力し、ピンポイントの比較ができるようにする
		/// </summary>
		/// <param name="mth">該当メソッド</param>
		/// <param name="leftRight"> "L" もしくは "R" を指定する（出力ファイル名のサフィックスになる）</param>
		private void outputOriginalMethodFile(MethodVO mth, string leftRight) {

			//BOM無しのUTF8でテキストファイルを作成する
			StreamWriter mthsw = new StreamWriter(outputDir + "\\detail\\#method_" + mth.guid.Substring(1,36) + "_" + leftRight + ".xml");
			mthsw.WriteLine( @"<?xml version=""1.0"" encoding=""utf-8""?> " );
			mthsw.WriteLine( "" );

			outputMethod(mth, 0, mthsw);

			mthsw.Close();
		}
		
		
		private Boolean compareNullable(string l, string r) {
			// 左が null の場合
			if( l == null ) {
				// 右も null なら true
				if ( r == null ) {
					return true;
				} else {
					// 右が not null なら false
					return false;
				}
			} else {
				// 左が not null の場合
				
				// 右が null なら false
				if ( r == null ) {
					return false;
				} else {
					// 両方 not null なので、string#Equalsの結果を返却
					return l.Equals(r);
				}
			}
			
		}
		
		
		private void outputConsole( ArtifactVO atf ) {
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

		
		public void outputMerged() {
			Console.WriteLine("outputMerged: outputDir=" + this.outputDir);
			
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
					outputChangedArtifactList(atf, listsw);
					
					//BOM無しのUTF8でテキストファイルを作成する
					StreamWriter atfsw = new StreamWriter(outputDir + "\\atf_" + atf.guid.Substring(1,36) + ".xml");
					atfsw.WriteLine( @"<?xml version=""1.0"" encoding=""utf-8""?> " );
					atfsw.WriteLine( "" );

					if( atf.package != null ) {
						atfsw.Write( "  <artifact " );
						atfsw.Write( " changed='" + atf.changed + "' " );
						atfsw.Write( " guid='" + atf.guid + "' " );
						atfsw.Write( " name='" + escapeXML(atf.name) + "' " );
						atfsw.Write( " path='" + escapeXML(atf.pathName) + "' " );
						atfsw.Write( " project='asw' " );
						atfsw.Write( " stereotype='" + atf.stereoType + "' " );
						atfsw.WriteLine( ">" );
						
						//sw.WriteLine("■成果物(" + atf.changed + "): GUID="+ atf.guid + ", Name=" + atf.name );
						outputChangedPackage(atf.package, 0, atfsw);
						
						atfsw.WriteLine( "  </artifact>" );
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
			chdesc = chdesc + "method:" + elem.methods.Count;

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
	
			sw.Write(indent(depth) + "</package>");
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
//			if (elemvo.taggedValues.Count > 0) {
//				outputClassTags(elemvo, depth+1, sw);
//			}
	
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
//		private void outputClassTags( ElementVO currentElement, int depth, StreamWriter sw ) {
//	
//			if ( currentElement.TaggedValues.Count <= 0 ) {
//				return ;
//			}
//	
//			sw.Write( indent(depth) + "<taggedValues>" );
//	
//			// 　取得できたタグ付き値の情報をファイルに展開する
//			foreach( TaggedValueVO tagv : currentElement.taggedValues ) {
//				if ("<memo>".Equals(tagv.Value)) {
//					if (tagv.Notes != null) {
//						sw.WriteLine( indent(depth) + "  <tv guid='" + escapeXML(tagv.TaggedValueGUID) + "' name='" + escapeXML(tagv.Name) + "' value='" + escapeXML(tagv.Value) + "'>" + escapeXML(tagv.Notes) + "</tv>" );
//					}
//				} else {
//					sw.WriteLine( indent(depth) + "  <tv guid='" + escapeXML(tagv.TaggedValueGUID) + "' name='" + escapeXML(tagv.Name) + "' value='" + escapeXML(tagv.Value) + "'/>" );
//				}
//			}
//	
//			sw.WriteLine( indent(depth) + "</taggedValues>" );
//		}


		// 1.2.2 クラスの属性を出力
		private void outputClassAttributes( ElementVO currentElement, int depth, StreamWriter sw ) {
	
			if ( currentElement.attributes.Count <= 0 ) {
				return;
			}
	
			//　取得できた属性の情報をファイルに展開する
			foreach(AttributeVO m_Att in currentElement.attributes) {
				outputAttribute(m_Att, depth, sw);
			}
	
		}

		private void outputAttribute(AttributeVO att, int depth, StreamWriter sw ) {
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
//				outputMethodParams( mth, depth, sw );
	
				if (mth.notes != null) {
					sw.WriteLine( indent(depth) + "  <notes>" + escapeXML( mth.notes ) + "</notes>" );
				}
	
				if (mth.behavior != null) {
					sw.WriteLine( indent(depth) + "  <behavior>" + escapeXML( mth.behavior ) + "</behavior>" );
				}
	
				// 1.2.3.1.2 メソッドのタグ付き値出力
				// Call outputMethodTags( mth, resp );
	
				sw.WriteLine( indent(depth) + "</method>" );

		}
		
		
		//  1.2.3.1.1 メソッドのパラメータ出力
//		private void outputMethodParams(MethodVO mth, int depth, StreamWriter sw) {
//			MethodParameterVO prm;
//	
//			if (mth.Parameters.Count <= 0) {
//				return ;
//			}
//	
//			// メソッドのパラメータ
//			if (mth.Parameters.Count > 0) {
//				foreach( MethodParamVO prm : mth.Parameters ) {
//					sw.Write( indent(depth) + "  <parameter " );
//					sw.Write( "guid='" + escapeXML(prm.guid) + "' " );
//					sw.Write( "name='" + escapeXML(prm.name) + "' " );
//					sw.Write( "type='" + escapeXML(prm.eaType) + "' " );
//					sw.Write( "alias='" + escapeXML(prm.alias) + "' " );
//					sw.Write( "pos='" + prm.pos + "' " 
//					sw.Write( ">" );
//					
//					if (prm.Notes != null) {
//						sw.WriteLine( indent(depth) + "  <notes>" + escapeXML( prm.notes ) + "</notes>");
//					}
//	
//					sw.WriteLine( indent(depth) + "  </parameter>" );
//				}
//			}
//	
//		}
	
		
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

	}
}
