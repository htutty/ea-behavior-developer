/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/06/01
 * Time: 13:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

using BDFileReader.vo;
using EA;

namespace BDFileReader.util
{
	/// <summary>
	/// Description of ObjectEAConverter.
	/// </summary>
	public class ObjectEAConverter
	{
		public ObjectEAConverter()
		{
		}
		

		public static PackageVO getPackageFromEAObject(EA.Package eaPackageObj) {
			PackageVO pacvo = new PackageVO();
			
			pacvo.packageId = eaPackageObj.PackageID ;
			pacvo.guid = eaPackageObj.PackageGUID ;
			pacvo.name = excludeSpecialChar(eaPackageObj.Name);
			pacvo.alias = excludeSpecialChar(eaPackageObj.Alias);
			pacvo.stereoType = excludeSpecialChar(eaPackageObj.StereotypeEx);
			pacvo.treePos = eaPackageObj.TreePos ;
			pacvo.isControlled = eaPackageObj.IsControlled;

			List<ElementVO> retElementList = new List<ElementVO>();
			foreach( EA.Element elemObj in eaPackageObj.Elements ) {
				retElementList.Add(getElementFromEAObject(elemObj));
			}
			
			List<PackageVO> retPackageList = new List<PackageVO>();
			foreach( EA.Package subPackObj in eaPackageObj.Packages ) {
				retPackageList.Add(getPackageFromEAObject(subPackObj));
			}
			
			pacvo.elements = retElementList;
			pacvo.childPackageList = retPackageList;
			return pacvo;			
		}


		public static ElementVO getElementFromEAObject(EA.Element eaElementObj) {
			ElementVO elemvo = new ElementVO();
			
			elemvo.name = excludeSpecialChar(eaElementObj.Name);
			elemvo.notes = eaElementObj.Notes;
			elemvo.guid = eaElementObj.ElementGUID;
			elemvo.eaType = eaElementObj.Type;
			elemvo.alias = excludeSpecialChar(eaElementObj.Alias);
			elemvo.stereoType = excludeSpecialChar(eaElementObj.StereotypeEx);
			elemvo.treePos = eaElementObj.TreePos;
			
			// EA内のタイプが "Class" で、かつ tag に値がセットされている場合は ElementRef情報を取得
			if ( "Class".Equals(eaElementObj.Type) && eaElementObj.Tag != null ) {
				elemvo.elementReferenceVO = getElementRefFromEAObject(eaElementObj);
			}
			
			getElementContentFromEAObject(eaElementObj, elemvo);
			
			return elemvo;
		}
		
		private static ElementReferenceVO getElementRefFromEAObject(EA.Element eaElementObj) {
			ElementReferenceVO outElemRef = new ElementReferenceVO();

			outElemRef.gentype = excludeSpecialChar(eaElementObj.Gentype);
			outElemRef.fqcn = excludeSpecialChar(eaElementObj.Tag);
			outElemRef.genfile = excludeSpecialChar(eaElementObj.Genfile);

			return outElemRef;
		}


		private static void getElementContentFromEAObject(EA.Element eaElementObj, ElementVO elemvo) {
			
			List<AttributeVO> retAttrList = new List<AttributeVO>();
			List<MethodVO> retMethList = new List<MethodVO>();
			List<TaggedValueVO> retTagValList = new List<TaggedValueVO>();
			
			// attributesの読み込み
			foreach( EA.Attribute attrObj in eaElementObj.Attributes) {
				retAttrList.Add(getAttributeFromEAObject(attrObj));
			}
			
			foreach( EA.Method mthObj in eaElementObj.Methods) {
				retMethList.Add(getMethodFromEAObject(mthObj));
			}
			
			foreach( EA.TaggedValue tagvObj in eaElementObj.TaggedValues) {
				retTagValList.Add(getTaggedValueFromEAObject(tagvObj));
			}

			elemvo.attributes = retAttrList;
			elemvo.attributes.Sort();

			elemvo.methods = retMethList;
			elemvo.methods.Sort();
			
			elemvo.taggedValues = retTagValList;
			elemvo.taggedValues.Sort();
		}

		
		public static AttributeVO getAttributeFromEAObject(EA.Attribute eaAttributeObj) {
			AttributeVO attvo = new AttributeVO();
			
			attvo.name = excludeSpecialChar(eaAttributeObj.Name);
			attvo.alias = excludeSpecialChar(eaAttributeObj.Alias);
			if ( eaAttributeObj.StereotypeEx != null && !"".Equals(eaAttributeObj.StereotypeEx) ) {
				attvo.stereoType = excludeSpecialChar(eaAttributeObj.StereotypeEx);
			} else {
				attvo.stereoType = null;
			}
			attvo.eaType = eaAttributeObj.Type;
			attvo.notes = eaAttributeObj.Notes;
			attvo.guid = eaAttributeObj.AttributeGUID;
			attvo.pos = eaAttributeObj.Pos;
			attvo.allowDuplicates = eaAttributeObj.AllowDuplicates;
			attvo.length = convStringToInt(eaAttributeObj.Length);
			attvo.classifierID = eaAttributeObj.ClassifierID.ToString();
			attvo.container = eaAttributeObj.Container;
			attvo.containment = eaAttributeObj.Containment;
			attvo.isDerived = eaAttributeObj.IsDerived;
			attvo.isID = eaAttributeObj.IsID;
			attvo.lowerBound = convStringToInt(eaAttributeObj.LowerBound);
			attvo.upperBound = convStringToInt(eaAttributeObj.UpperBound);
			attvo.precision = convStringToInt(eaAttributeObj.Precision);
			attvo.scale = convStringToInt(eaAttributeObj.Scale);
			attvo.visibility = excludeSpecialChar(eaAttributeObj.Visibility);
			
			List<AttributeTagVO> outTagList = new List<AttributeTagVO>();
			foreach( EA.AttributeTag atag in eaAttributeObj.TaggedValuesEx ) {
				outTagList.Add(getAttributeTagFromEAObject(atag));
			}
			attvo.taggedValues = outTagList;
			
			return attvo;
		}
		

		public static AttributeTagVO getAttributeTagFromEAObject(EA.AttributeTag eaAttributeTagObj) {
			AttributeTagVO atvvo = new AttributeTagVO();
			atvvo.name = excludeSpecialChar(eaAttributeTagObj.Name);
			atvvo.guid = eaAttributeTagObj.TagGUID;
			atvvo.tagValue = excludeSpecialChar(eaAttributeTagObj.Value);
			atvvo.notes = eaAttributeTagObj.Notes;
			atvvo.changed = ' ';
			return atvvo;
		}



		private static int convStringToInt(string src) {
			int ret;
			
			if (!Int32.TryParse(src, out ret)) {
				ret = 0;
			}

			return ret;
		}
		
		
		
		public static MethodVO getMethodFromEAObject(EA.Method eaMethodObj) {
			MethodVO mthvo = new MethodVO();
			
			mthvo.name = excludeSpecialChar(eaMethodObj.Name);
			mthvo.alias = excludeSpecialChar(eaMethodObj.Alias);
			if ( eaMethodObj.StereotypeEx != null && !"".Equals(eaMethodObj.StereotypeEx) ) {
				mthvo.stereoType = eaMethodObj.StereotypeEx;
			} else {
				mthvo.stereoType = null;
			}
			
			mthvo.guid = eaMethodObj.MethodGUID;
			mthvo.pos = eaMethodObj.Pos;
			mthvo.classifierID = eaMethodObj.ClassifierID;
			mthvo.isConst = eaMethodObj.IsConst;
			mthvo.isLeaf = eaMethodObj.IsLeaf;
			mthvo.isPure = eaMethodObj.IsPure;
			mthvo.isQuery = eaMethodObj.IsQuery;
			mthvo.isRoot = eaMethodObj.IsRoot;
			mthvo.returnIsArray = eaMethodObj.ReturnIsArray;
			mthvo.stateFlags = eaMethodObj.StateFlags;
			mthvo.behavior = eaMethodObj.Behavior;
			mthvo.notes = eaMethodObj.Notes;
			mthvo.returnType = excludeSpecialChar(eaMethodObj.ReturnType);
			mthvo.visibility = excludeSpecialChar(eaMethodObj.Visibility);
			
			List<ParameterVO> outParamList = new List<ParameterVO>();
			foreach(EA.Parameter eaParamObj in eaMethodObj.Parameters) {
				ParameterVO prm = new ParameterVO();
				prm.name = excludeSpecialChar(eaParamObj.Name);
				prm.alias = excludeSpecialChar(eaParamObj.Alias);
				prm.eaType = excludeSpecialChar(eaParamObj.Type);
				prm.stereoType = excludeSpecialChar(eaParamObj.StereotypeEx);
				prm.guid = eaParamObj.ParameterGUID;
				prm.pos = eaParamObj.Position;
				prm.classifierID = eaParamObj.ClassifierID;
				prm.defaultValue = excludeSpecialChar(eaParamObj.Default);
				prm.isConst = eaParamObj.IsConst;
				prm.kind = eaParamObj.Kind;
				prm.styleEx = excludeSpecialChar(eaParamObj.StyleEx);
				prm.notes = eaParamObj.Notes;
				prm.objectType = "25";
				outParamList.Add(prm);
			}
			mthvo.parameters = outParamList;

			List<MethodTagVO> outTagList = new List<MethodTagVO>();
			foreach( EA.MethodTag mtag in eaMethodObj.TaggedValues ) {
				outTagList.Add(getMethodTagFromEAObject(mtag));
			}
			mthvo.taggedValues = outTagList;

			return mthvo;
		}
		
		
		public static MethodTagVO getMethodTagFromEAObject(EA.MethodTag eaMethodTagObj) {
			MethodTagVO mtvvo = new MethodTagVO();
			mtvvo.name = excludeSpecialChar(eaMethodTagObj.Name);
			mtvvo.guid = eaMethodTagObj.TagGUID;
			mtvvo.tagValue = excludeSpecialChar(eaMethodTagObj.Value);
			mtvvo.notes = eaMethodTagObj.Notes;
			mtvvo.changed = ' ';
			return mtvvo;
		}
		
		
		private static TaggedValueVO getTaggedValueFromEAObject(EA.TaggedValue eaTagObj) {
			TaggedValueVO tvvo = new TaggedValueVO();
			tvvo.name = excludeSpecialChar(eaTagObj.Name);
			tvvo.guid = eaTagObj.PropertyGUID;
			tvvo.tagValue = excludeSpecialChar(eaTagObj.Value);
			tvvo.notes = eaTagObj.Notes;
			tvvo.changed = ' ';
			return tvvo;
		}

		
		public static bool updateEAMethod(MethodVO mth, Method eaMethodObj) {
			eaMethodObj.Name = excludeSpecialChar(mth.name);
			eaMethodObj.Behavior = mth.behavior;
			eaMethodObj.Notes = mth.notes;
			return eaMethodObj.Update();
		}

		
		private static string excludeSpecialChar(string str) {

			if( str == null || str == "" ) {
				return "";
			}
			
			char[] ary = str.ToCharArray();
			string retstr = "";
			
			for( int i=0; i < ary.Length; i++ ) {
				// 文字コード 0x20（半角スペース）よりも小さい場合は制御文字とみなす
				if( ary[i] < 0x20 ) {
					Console.WriteLine("制御文字 : " + ary[i] );
				} else {
					retstr = retstr + ary[i];
				}
			}
			
			return retstr;
		}

	}
}
