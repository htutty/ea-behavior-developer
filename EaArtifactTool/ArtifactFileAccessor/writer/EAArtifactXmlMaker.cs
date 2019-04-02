using System;
using EA;

using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;

namespace ArtifactFileAccessor.writer
{
	/// <summary>
	/// Description of EAArtifactXmlMaker.
	/// </summary>
	public class EAArtifactXmlMaker
	{
		EA.Package rootPackObj = null;
		
		public EAArtifactXmlMaker(EA.Package packObj) 
		{
			rootPackObj = packObj;
		}
		
		
		/// <summary>
		/// EAから取得したパッケージ情報でartifactXMLを更新し、取得結果を新しいArtifactVOに入れて返却する
		/// </summary>
		/// <returns></returns>
		public ArtifactVO makeArtifactXml() {
//			if (eaPackageObj.IsControlled == false) {
//			}
			ArtifactVO atf = new ArtifactVO();
			atf.artifactId = "";
			atf.changed = ' ';
			atf.createDate = rootPackObj.Element.Created.ToString();
			atf.guid = rootPackObj.PackageGUID;
			atf.name = rootPackObj.Name;
			atf.notes = rootPackObj.Notes;
			atf.pathName = makePackagePathFromEAPackage(rootPackObj);
			atf.projectName = "ASW";
			atf.stereoType = rootPackObj.StereotypeEx;
			atf.updateDate = rootPackObj.Element.Modified.ToString();

			atf.package = ObjectEAConverter.getPackageFromEAObject(rootPackObj);
			atf.packageId = rootPackObj.PackageID;

            string artifactDir = ProjectSetting.getVO().projectPath + ProjectSetting.getVO().artifactsPath;
            ArtifactXmlWriter.outputArtifactXml(artifactDir, atf);
//			writer.outputArtifactXml(@"C:\WORK", atf);

			return atf;
		}

		
		private string makePackagePathFromEAPackage(EA.Package pacObj) {
			EA.Repository repo = ProjectSetting.getEARepo();
			
			if(pacObj.ParentID == 0) {
				return "/" + pacObj.Name ;
			} else {
				EA.Package parentPacObj = repo.GetPackageByID(pacObj.ParentID);
				return makePackagePathFromEAPackage(parentPacObj) + "/" + pacObj.Name;
			}
			
		}
		
		
	}
}
