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
		private string fromProjectDir = null;
		private string toProjectDir = null;
		
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
		public ArtifactsDiffer(string fromProjectFile_, string toProjectFile_) {
			if ( fromProjectFile_ != null ) {
				this.fromProjectDir = Path.GetDirectoryName(fromProjectFile_);
			}
			
			if ( toProjectFile_ != null ) {
				this.toProjectDir = Path.GetDirectoryName(toProjectFile_);
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
						outList.Add(outAtf);
						rCnt++;
					} else {
						outAtf = lAtf;
						outAtf.changed = 'D';
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
			for (lCnt=0, rCnt=0; lCnt < leftPkg.childPackageList.Count && rCnt < rightPkg.childPackageList.Count; ) {
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
						outPackageList.Add(oPkg);
						rCnt++;
					} else {
						oPkg = lPkg;
						oPkg.changed = 'D';
						outPackageList.Add(oPkg);
						lCnt++;
					}
				}
			}
			outPkg.childPackageList = outPackageList;

			
			// パッケージが保持する要素リストの比較
			ElementVO lElm, rElm, outElm;
			List<ElementVO> outElements = new List<ElementVO>();
			for (lCnt=0, rCnt=0; lCnt < leftPkg.elements.Count && rCnt < rightPkg.elements.Count; ) {
				lElm = leftPkg.elements[lCnt];
				rElm = rightPkg.elements[rCnt];
				
				// GUIDで一致するものがあった場合: 
				if (compareElementGuid(lElm, rElm) == 0) {
					outElm = getDisagreedElement(lElm, rElm);
					if (outElm != null) {
						outElm.changed = 'U';
						outElements.Add(outElm);
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


		private int compareAttributeGuid(AttributeVO leftAtr, AttributeVO rightAtr) {
			return leftAtr.guid.CompareTo(rightAtr.guid);
		}

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
//			if( !leftElem.toDescriptorString().Equals(rightElem.toDescriptorString()) ) {
//				return rightElem;
//			} else {
//				return null;
//			}

			ElementVO outElm;
			if( !leftElm.name.Equals(rightElm.name) ) {
				outElm = rightElm;
				outElm.changed = 'U';
			} else {
				outElm = leftElm;
				outElm.changed = ' ';
			}

			Int16 lCnt, rCnt;


			// 要素が保持する属性リストの比較
			AttributeVO lAtr, rAtr, oAtr;
			List<AttributeVO> outAttributeList = new List<AttributeVO>();
			for (lCnt=0, rCnt=0; lCnt < leftElm.attributes.Count && rCnt < rightElm.attributes.Count; ) {
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
						rCnt++;
					} else {
						oAtr = lAtr;
						oAtr.changed = 'D';
						outAttributeList.Add(oAtr);
						lCnt++;
					}
				}
			}
			outElm.attributes = outAttributeList;

			
			// 要素が保持するメソッドリストの比較			
			MethodVO lMth, rMth, outMth;
			List<MethodVO> outMethods = new List<MethodVO>();
			for (lCnt=0, rCnt=0; lCnt < leftElm.methods.Count && rCnt < rightElm.methods.Count; ) {
				lMth = leftElm.methods[lCnt];
				rMth = rightElm.methods[rCnt];
				
				// GUIDで一致するものがあった場合: 
				if (compareMethodGuid(lMth, rMth) == 0) {
					outMth = getDisagreedMethod(lMth, rMth);
					if (outMth != null) {
						outMth.changed = 'U';
						outMethods.Add(outMth);
					}
					
					lCnt++;
					rCnt++;
				} else {
					// GUIDで一致するものが無かった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
					if(compareMethodGuid(lMth, rMth) > 0) {
						rMth.changed = 'C';
						outMethods.Add(rMth);
						rCnt++;
					} else {
						lMth.changed = 'D';
						outMethods.Add(lMth);
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
			
			outAtr = leftAtr;
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
			
			if( leftAtr.pos != rightAtr.pos ) {
				outAtr.pos = rightAtr.pos;
				outAtr.changed = 'U';
			} 

			if( !compareNullable(leftAtr.notes, rightAtr.notes) ) {
				outAtr.notes = leftAtr.notes + "\n------ ↓ ↓ ↓ ↓ ------\n" + rightAtr.notes;
				outAtr.changed = 'U';
			} 
			
			
			if ( outAtr.changed == ' ' ) {
				return null;
			} else {
				return outAtr ;
			}
			
		}
		
		
		
		private MethodVO getDisagreedMethod(MethodVO leftMth, MethodVO rightMth) {
			MethodVO outMth;
			
			outMth = leftMth;
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
			
			if( leftMth.pos != rightMth.pos ) {
				outMth.pos = rightMth.pos;
				outMth.changed = 'U';
			} 

			if( !compareNullable(leftMth.notes, rightMth.notes) ) {
				outMth.notes = leftMth.notes + "\n------ ↓ ↓ ↓ ↓ ------\n" + rightMth.notes;
				outMth.changed = 'U';
			}
			
			if( !compareNullable(leftMth.behavior, rightMth.behavior) ) {
				outMth.behavior = leftMth.behavior + "\n------ ↓ ↓ ↓ ↓ ------\n" + rightMth.behavior;
				outMth.changed = 'U';
			}
						
			
			if ( outMth.changed == ' ' ) {
				return null;
			} else {
				return outMth ;
			}
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

		public void outputMerged(string outputDir) {
			foreach (ArtifactVO atf in this.outArtifacts) {
				Console.WriteLine("■成果物(" + atf.changed + "): GUID="+ atf.guid + ", Name=" + atf.name );
				if( atf.package != null ) {
					dumpChangedPackageContents(atf.package);
				}
			}
			
			
		}

		private void dumpChangedPackageContents(PackageVO pkgvo) {
			Console.WriteLine("  □パッケージ(" + pkgvo.changed + "): GUID="+ pkgvo.guid + ", Name=" + pkgvo.name );

			foreach (PackageVO c in pkgvo.childPackageList) {
				dumpChangedPackageContents(c);
			}

			foreach (ElementVO elm in pkgvo.elements) {
				dumpChangedElementContents(elm);
			}
			
		}

		private void dumpChangedElementContents(ElementVO elmvo) {
			Console.WriteLine("  ○クラス(" + elmvo.changed + "): GUID="+ elmvo.guid + ", Name=" + elmvo.name );
			Console.WriteLine( elmvo.toDescriptorString() );
		}
	}
}
