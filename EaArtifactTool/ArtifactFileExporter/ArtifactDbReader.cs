﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using ArtifactFileAccessor.vo;
using ArtifactFileAccessor.util;
using System.IO;

namespace ArtifactFileExporter
{
    public class ArtifactDbReader
    {
        // 全メソッドのレコードを読み込み、要素IDごとにハッシュに格納したもの
        private Dictionary<int, List<MethodVO>> g_AllMethodsInElementMap = new Dictionary<int, List<MethodVO>>();
        // 全属性のレコードを読み込み、要素IDごとにハッシュに格納したもの
        private Dictionary<int, List<AttributeVO>>  g_AllAttributesInElementMap = new Dictionary<int, List<AttributeVO>>();

        // 全パッケージVO のキャッシュ用Map
        public Dictionary<int, PackageVO> AllPackageMap = new Dictionary<int, PackageVO>();

        List<PackageVO> rootPackages = new List<PackageVO>();

        // ルートパッケージ名の保持
        private string rootName;

        // DBのコネクション
        private OleDbConnection objConn;


        #region コンストラクタと事前データ準備

        /// <summary>
        /// パラメータの無いコンストラクタ(privateのため呼び出し不可)
        /// </summary>
        private ArtifactDbReader()
        {

        }

        /// <summary>
        /// OleDbConnectionをパラメータとするコンストラクタ
        /// </summary>
        /// <param name="conn">DBコネクション</param>
        public ArtifactDbReader(OleDbConnection conn)
        {
            this.objConn = conn;

            getAllAttributeMap();
            getAllMethodMap();

        }

        /// <summary>
        /// 全属性のMap（ディクショナリ）を作成
        /// </summary>
        private void getAllAttributeMap()
        {
            Console.WriteLine("getAllAttributeMap()");

            string strSQL, strFields;

	        // 読み込む項目リスト
	        strFields =
		        "att.ID, att.ea_guid, att.Name, att.Style, att.Notes, att.[Default], " +
		        "att.IsCollection, att.Const, att.IsOrdered, att.IsStatic, att.Length, " +
		        "att.Stereotype, att.Type, att.Scope, att.Pos, att.Object_ID " ;

            // SQL文 を作成
            strSQL = "select " + strFields +
		        " from t_attribute att " +
		        " order by att.Object_ID, att.ea_guid " ;

	        OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
	        dbCom.Connection = objConn;

            var attrList = new List<AttributeVO>();
            OleDbDataReader reader = dbCom.ExecuteReader();

            while (reader.Read())
            {
                AttributeVO retAtt = new AttributeVO();

                retAtt.attributeId = DbUtil.readIntField(reader, 0);
                retAtt.guid = DbUtil.readStringField(reader, 1);
                retAtt.name = StringUtil.excludeSpecialChar("t_attribute", "name", retAtt.guid, DbUtil.readStringField(reader, 2));
                retAtt.alias = DbUtil.readStringField(reader, 3);
                retAtt.notes = DbUtil.readStringField(reader, 4);
                retAtt.defaultValue = DbUtil.readStringField(reader, 5);

		        retAtt.isCollection = DbUtil.readBooleanIntField(reader, 6);
		        retAtt.isConst = DbUtil.readBooleanIntField(reader, 7);
		        retAtt.isOrdered = DbUtil.readBooleanIntField(reader, 8);
		        retAtt.isStatic = DbUtil.readBooleanIntField(reader, 9);
		        retAtt.length = DbUtil.readIntField(reader, 10);
		        retAtt.objectType = "23";
                retAtt.stereoType = DbUtil.readStringField(reader, 11);
                retAtt.eaType = DbUtil.readStringField(reader, 12);
                retAtt.visibility = DbUtil.readStringField(reader, 13);
		        retAtt.pos = DbUtil.readIntField(reader, 14);
		        retAtt.elementId = DbUtil.readIntField(reader, 15);

		        attrList.Add(retAtt);
	        }

            reader.Close();
            Console.WriteLine("--- AllAttributesInElementMap 作成処理");

	        int saveElementID = 0;
            var attributeListInElem = new List<AttributeVO>();
	        foreach(AttributeVO att in attrList) {
		        // WScript.Echo "attribute: [ElemId=" & att.ElementID & ", AttributeID=" & att.AttributeID & "]"
		        if(saveElementID == 0) {
                    attributeListInElem = new List<AttributeVO>();
                    saveElementID = att.elementId;
		        }

		        // 要素IDが前行と同一の場合
		        if (saveElementID == att.elementId ) {
			        // 現在のメソッドリストにこのメソッドを追加
			        attributeListInElem.Add(att);
		        } else {
			        // 現在のメソッドリストを返却Mapに追加
			        g_AllAttributesInElementMap.Add(saveElementID, attributeListInElem);

			        attributeListInElem = new List<AttributeVO>();
			        attributeListInElem.Add(att);

			        saveElementID = att.elementId;
		        }
	        }

	        // 最後に溜まっている分の要素内操作リストを返却Mapに追加
	        g_AllAttributesInElementMap.Add(saveElementID, attributeListInElem);


            attrList.Sort();
            // 属性のタグ付き値を AttributeVO.taggedValues にセットする
            setAttributeTag(attrList);

        }

        /// <summary>
        /// 属性のタグ付き値を読み込み
        /// </summary>
        /// <param name="attributes"></param>
        private void setAttributeTag(List<AttributeVO> attributes)
        {
            Console.WriteLine("setAttributeTag(): attributes.Count=" + attributes.Count);

            int attrIdx = 0;
            int attributeId, savedAttributeId = 0;
            string strSQL, strFields;

            // 読み込む項目リスト
            strFields =
              "ElementID, PropertyID, ea_guid, Property, VALUE, NOTES ";

            // SQL文 を作成
            strSQL = "select " + strFields +
                " from t_attributetag " +
                " order by ElementID, Property ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            var retAttrTags = new List<AttributeTagVO>();
            OleDbDataReader reader = dbCom.ExecuteReader();

            while (reader.Read())
            {
                attributeId = DbUtil.readIntField(reader, 0);

                AttributeTagVO atag = new AttributeTagVO();

                atag.guid = DbUtil.readStringField(reader, 2);
                atag.name = StringUtil.excludeSpecialChar("t_attributetag", "Property", atag.guid, DbUtil.readStringField(reader, 3));
                atag.tagValue = DbUtil.readStringField(reader, 4);
                atag.notes = DbUtil.readStringField(reader, 5);

                if (savedAttributeId > 0 && savedAttributeId < attributeId)
                {
                    attrIdx = searchAttributeAndSetTags(attributes, savedAttributeId, attrIdx + 1, retAttrTags);
                    retAttrTags = new List<AttributeTagVO>();
                }

                retAttrTags.Add(atag);
                savedAttributeId = attributeId;
            }

            // 最後に retAttrTags に溜まっている分をメソッドにセット
            if (savedAttributeId > 0)
            {
                attrIdx = searchAttributeAndSetTags(attributes, savedAttributeId, attrIdx + 1, retAttrTags);
            }

            reader.Close();
        }

        /// <summary>
        /// 引数の属性リストから属性IDでヒットする属性を探し、そのtaggedValuesに取得した属性タグ付き値のリストをセットする
        /// </summary>
        /// <param name="attributes">属性リスト</param>
        /// <param name="targetAttrId">属性ID（t_attributetagから取得）</param>
        /// <param name="startIdx">開始インデックス</param>
        /// <param name="atags">属性のタグ付き値リスト</param>
        /// <returns></returns>
        private int searchAttributeAndSetTags(List<AttributeVO> attributes, int targetAttrId, int startIdx, List<AttributeTagVO> atags)
        {
            for (int i = startIdx; i < attributes.Count; i++)
            {
                AttributeVO attr = attributes[i];
                if (attr.attributeId == targetAttrId)
                {
                    attr.taggedValues = atags;
                    return i;
                }
            }

            return startIdx;
        }


        /// <summary>
        /// 全メソッドのMap（ディクショナリ）を作成
        /// </summary>
        private void getAllMethodMap()
        {

            Console.WriteLine("getAllMethodMap()");

            string strSQL, strFields;

            // 読み込む項目リスト
            strFields =
                " ope.OperationID, ope.ea_guid,    ope.Name,        ope.Style,       ope.Notes,    " +
                " ope.Behaviour,   ope.Code,       ope.[Type],      ope.Abstract,    ope.IsStatic, " +
                " ope.Object_ID,   ope.Stereotype, ope.Scope,       ope.Pos ";

            // SQL文 を作成
            strSQL = "select " + strFields +
                " from t_operation ope " +
                " order by ope.Object_ID, ope.Pos, ope.Name ";


            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            int i = 0;
            MethodVO retMth;
            List<MethodVO> mthList = new List<MethodVO>();

            OleDbDataReader reader = dbCom.ExecuteReader();

            // DBから取得したメソッドデータをいったんリストに貯める
            while (reader.Read())
            {
                retMth = new MethodVO();
                retMth.methodId = DbUtil.readIntField(reader, 0);
                retMth.guid = DbUtil.readStringField(reader, 1);
                retMth.name = StringUtil.excludeSpecialChar("t_operation", "name", retMth.guid, DbUtil.readStringField(reader, 2));
                retMth.alias = StringUtil.excludeSpecialChar("t_operation", "alias", retMth.guid, DbUtil.readStringField(reader, 3));
                retMth.notes = DbUtil.readStringField(reader, 4);
                retMth.behavior = DbUtil.readStringField(reader, 5);
                retMth.code = DbUtil.readStringField(reader, 6);
                retMth.returnType = DbUtil.readStringField(reader, 7);
                retMth.isAbstract = DbUtil.readBooleanStringField(reader, 8);
                retMth.isStatic = DbUtil.readBooleanStringField(reader, 9);
                retMth.objectType = "21";
                retMth.elementId = DbUtil.readIntField(reader, 10);

                retMth.stereoType = DbUtil.readStringField(reader, 11);
                retMth.visibility = DbUtil.readStringField(reader, 12);
                retMth.pos = DbUtil.readIntField(reader, 13);

                mthList.Add(retMth);

                i = i + 1;
            }

            reader.Close();
            Console.WriteLine("--- AllMethodsInElementMap 作成処理");

            int saveElementID;
            var methodListInElem = new List<MethodVO>();
            saveElementID = 0;

            foreach (MethodVO mth in mthList)
            {
                Console.WriteLine("method: [ElemId=" + mth.elementId + ", MethodID=" + mth.methodId + "]");
                if (saveElementID == 0)
                {
                    methodListInElem = new List<MethodVO>();
                    saveElementID = mth.elementId;
                }

                // 要素IDが前行と同一の場合
                if (saveElementID == mth.elementId)
                {
                    // 現在のメソッドリストにこのメソッドを追加
                    methodListInElem.Add(mth);
                } else {
                    // 現在のメソッドリストを返却Mapに追加
                    g_AllMethodsInElementMap.Add(saveElementID, methodListInElem);

                    methodListInElem = new List<MethodVO> { mth };
                    saveElementID = mth.elementId;
                }
            }

            // 最後に溜まっている分の要素内操作リストを返却Mapに追加
            g_AllMethodsInElementMap.Add(saveElementID, methodListInElem);

            // 取得したメソッドにパラメータを突っ込む
            // operationId 順にソート
            mthList.Sort(new MethodIdComparer());

            // このメソッドのparametersを取得(なければmth.parametersに空のリストが設定される)
            setMethodParams(mthList);
        }


        /// <summary>
        /// メソッドパラメータを全件読み、operationID でヒットした引数のメソッドリストにセット
        /// </summary>
        /// <param name="methods">全メソッドのリスト</param>
        private void setMethodParams(List<MethodVO> methods)
        {
            Console.WriteLine("setMethodParams(): methods.Count=" + methods.Count);
            // 全メソッドのリストをソート（キー＝methodId）
            methods.Sort(new MethodIdComparer());

            int methodIdx = 0;
            int operationId, savedOperationId = 0;
            string strSQL, strFields;

            // 読み込む項目リスト
            strFields =
                " oprm.Name,       oprm.[Type],    oprm.[Default],  oprm.Notes,      oprm.Pos,     " +
                " oprm.Const,      oprm.Style,     oprm.Kind,       oprm.Classifier, oprm.ea_guid, " +
                " oprm.StyleEx,    oprm.OperationID ";

            // SQL文 を作成
            strSQL = "select " + strFields +
                " from t_operationparams oprm " +
                " where oprm.OperationID > 0 " +
                " order by oprm.OperationID, oprm.Pos, oprm.Name ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            // DBから取得したパラメータデータをいったんリストに貯める
            List<ParameterVO> retParameters = new List<ParameterVO>();
            OleDbDataReader reader = dbCom.ExecuteReader();

            while (reader.Read())
            {
                operationId = DbUtil.readIntField(reader, 11);

			    ParameterVO prm = new ParameterVO();

                prm.name = StringUtil.excludeSpecialChar("t_operationparams", "name", prm.guid, DbUtil.readStringField(reader, 0));
			    prm.eaType = DbUtil.readStringField(reader, 1);
			    prm.defaultValue = StringUtil.excludeSpecialChar("t_operationparams", "Default", prm.guid, DbUtil.readStringField(reader, 2));
			    prm.notes = DbUtil.readStringField(reader, 3);
			    prm.pos = DbUtil.readIntField(reader, 4);
			    prm.isConst = DbUtil.readBoolField(reader, 5);
			    prm.stereoType = DbUtil.readStringField(reader, 6);
			    prm.kind = DbUtil.readStringField(reader, 7);
			    prm.classifierID = DbUtil.readStringField(reader, 8);
                prm.guid = DbUtil.readStringField(reader, 9);
                string buf = DbUtil.readStringField(reader, 10);
			    if(buf.Length > 7) {
				    prm.alias = buf.Substring(7, buf.Length - 8);
			    } else {
				    prm.alias = buf;
			    }

                prm.styleEx = buf;
			    prm.objectType = "25";

                if (savedOperationId > 0 && savedOperationId < operationId)
                {
                    methodIdx = searchMethodAndSetParameter(methods, savedOperationId, methodIdx, retParameters);
                    retParameters = new List<ParameterVO>();
                }

                retParameters.Add(prm);
                savedOperationId = operationId;
            }

            // 最後に retParamtersに溜まっている分をメソッドにセット
            if (savedOperationId > 0)
            {
                methodIdx = searchMethodAndSetParameter(methods, savedOperationId, methodIdx + 1, retParameters);
            }

            reader.Close();

        }

        /// <summary>
        /// メソッドの全リストの中から引数のメソッドIDで合致するメソッドを見つけ、そこに引数のパラメータリストを紐づける
        /// </summary>
        /// <param name="methods">メソッドの全リスト</param>
        /// <param name="targetMethodId">メソッドID（パラメータテーブルから取得したID）</param>
        /// <param name="startIdx">開始インデックス</param>
        /// <param name="prms">パラメータリスト</param>
        /// <returns></returns>
        private int searchMethodAndSetParameter(List<MethodVO> methods, int targetMethodId, int startIdx, List<ParameterVO> prms)
        {
            // パラメータのリストをGUIDでソートするためのコンパレータ
            ParameterGuidComparer comp = new ParameterGuidComparer();

            for (int i=startIdx; i < methods.Count; i++)
            {
                MethodVO mth = methods[i];
                if(mth.methodId == targetMethodId)
                {
                    // メソッドにパラメータのリストをセットする際にGUIDでソートする
                    prms.Sort(comp);

                    mth.parameters = prms;
                    return i;
                }
            }

            return startIdx;
        }

        #endregion

        #region 成果物リスト生成
        /// <summary>
        /// 要素が内蔵された全パッケージリストから成果物リストを生成して返却する
        /// </summary>
        /// <param name="targetProject">プロジェクト名</param>
        /// <param name="targetModel">モデル名</param>
        /// <returns>成果物リスト情報</returns>
        public List<ArtifactVO> getArtifactList()
        {
            try {
                var retArtifactList = new List<ArtifactVO>();
                // ArtifactsVO artifacts = new ArtifactsVO();
                // artifacts.artifactList = new List<ArtifactVO>();

                // DBから全ルートパッケージ読み込み
                readAllRootPackages();

                Console.WriteLine("全てのルートパッケージから成果物パッケージを検索: ");
                foreach (PackageVO pkg in rootPackages)
                {
                    rootName = pkg.name;
                    searchArtifactPackage(pkg, "", retArtifactList);
                }

                return retArtifactList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message=" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// パッケージの子を辿り、成果物マーク(isControlled)が立っているパッケージを成果物としてリストに追加する
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="parentPath"></param>
        private void searchArtifactPackage(PackageVO pkg, string parentPath, List<ArtifactVO> outArtifacts)
        {
            string nowPath = parentPath + "/" + pkg.name;
            if (pkg.isControlled)
            {
                Console.WriteLine("hit artifactTarget: guid=" + pkg.guid + ", path=" + nowPath);

                // 成果物VOを生成し、このパッケージを対象にする
                ArtifactVO atf = new ArtifactVO();
                atf.guid = pkg.guid;
                atf.name = pkg.name;
                atf.pathName = parentPath;
                atf.projectName = rootName;
                atf.package = pkg;

                // 出力用の成果物リストに追加
                outArtifacts.Add(atf);
            }
            else
            {
                if (pkg.childPackageList != null)
                {
                    foreach (PackageVO p in pkg.childPackageList)
                    {
                        searchArtifactPackage(p, nowPath, outArtifacts);
                    }
                }
            }

        }
        #endregion

        #region 全ルートパッケージ読み込み
        /// <summary>
        /// ルート（parentIdが0）パッケージ情報を取得する
        /// </summary>
        /// <returns>ルートパッケージのリスト</returns>
        private List<PackageVO> readAllRootPackages()
        {
            Console.WriteLine("readAllRootPackages()");

            string strSQL;

            strSQL =
              "SELECT  pac.PACKAGE_ID " +
              "    ,pac.NAME " +
              "    ,pac.PARENT_ID " +
              "    ,pac.TPOS " +
              "    ,pac.ea_guid " +
              "    ,pac.IsControlled " +
              " FROM t_package pac " +
              " WHERE pac.PARENT_ID = 0 ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            using (OleDbDataReader reader = dbCom.ExecuteReader())
            {
                // 結果を表示します。
                while (reader.Read())
                {
                    PackageVO rootPack = new PackageVO();

                    rootPack.packageId = DbUtil.readIntField(reader, 0);
                    rootPack.guid = DbUtil.readStringField(reader, 4);
                    rootPack.name = StringUtil.excludeSpecialChar("t_package", "name", rootPack.guid, DbUtil.readStringField(reader, 1));
                    rootPack.alias = "";
                    rootPack.stereoType = "";
                    rootPack.treePos = DbUtil.readIntField(reader, 3);
                    rootPack.isControlled = DbUtil.readBoolField(reader, 5);
                    rootPack.pathName = rootPack.name;

                    // このパッケージ配下の要素を読み込み
                    readElements(rootPack);

                    // パッケージ配下のパッケージを読み込み（再帰処理）
                    readSubPackage(rootPack);

                    // 返却用のパッケージリストにこのパッケージを追加
                    rootPackages.Add(rootPack);

                    // 全パッケージマップのキャッシュに、Key=IDとして追加
                    AllPackageMap.Add(rootPack.packageId, rootPack);
                }
            }

            return rootPackages;
        }


        /// <summary>
        /// 親の存在するサブパッケージ配下のパッケージ、要素の読み込み
        /// </summary>
        /// <param name="parent"></param>
        private void readSubPackage(PackageVO parent)
        {
            Console.WriteLine("readSubPackage(): " + parent.name + "packageID =" + parent.packageId);

            string strSQL;

            strSQL =
              "SELECT  pac.PACKAGE_ID " +
              "       ,pac.NAME " +
              "       ,obj.Alias " +
              "       ,obj.Stereotype " +
              "       ,pac.ea_guid " +
              "       ,pac.PARENT_ID " +
              "       ,pac.TPOS " +
              "       ,pac.IsControlled " +
              " FROM t_package pac left outer join t_object obj on pac.ea_guid = obj.ea_guid " +
              " WHERE pac.PARENT_ID = " + parent.packageId + " " +
              " ORDER BY pac.TPOS, pac.NAME  " ;

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;


            List<PackageVO> retPackages = new List<PackageVO>();

            using(OleDbDataReader reader = dbCom.ExecuteReader())
            {

                // 結果を表示します。
                while (reader.Read())
                {
                    PackageVO pack = new PackageVO();

                    pack.packageId = DbUtil.readIntField(reader, 0);
                    pack.guid = DbUtil.readStringField(reader, 4);
                    pack.name = StringUtil.excludeSpecialChar("t_package", "name", pack.guid, DbUtil.readStringField(reader, 1));
                    pack.alias = StringUtil.excludeSpecialChar("t_object", "alias", pack.guid, DbUtil.readStringField(reader, 2));
                    pack.stereoType = DbUtil.readStringField(reader, 3);
                    pack.treePos = DbUtil.readIntField(reader, 6);
                    pack.isControlled = DbUtil.readBoolField(reader, 7);
                    pack.pathName = parent.pathName + "/" + pack.name;

                    // このパッケージ配下の要素読み込み
                    readElements(pack);

                    // このパッケージ配下のダイアグラム読み込み
                    readDiagrams(pack);

                    // このパッケージ配下の子パッケージ読み込み
                    readSubPackage(pack);

                    retPackages.Add(pack);

                    // 全パッケージマップのキャッシュに、Key=IDとして追加
                    AllPackageMap.Add(pack.packageId, pack);
                }
            }

            parent.childPackageList = retPackages;
        }

        /// <summary>
        /// パッケージ配下の要素を読み込み、パッケージオブジェクトのelementsにセットする
        /// </summary>
        /// <param name="package">対象パッケージオブジェクト</param>
        private void readElements(PackageVO package)
        {
            //	WScript.Echo "Start ExportElements"
            Console.WriteLine("readElements(): packageID =" + package.packageId);

            string strSQL, strFields, strWhere;

            // 読み込む項目リスト
            strFields =
                "Object_ID, ea_guid, Alias, Author, CreatedDate, ModifiedDate, " +
                "Name, Note, NType, Package_ID, ParentID, Stereotype, PDATA5, " +
                "TPos, Object_Type, Version, Visibility, GenType,  GenFile ";

            // WHERE文: DBに対しては主に パッケージID で検索（キー＝IDで結合する必要があるため）
            strWhere = " where  Package_ID = " + package.packageId + " " +
                 " AND Object_Type IN ('GUIElement', 'Screen', 'Class', 'Interface', 'Enumeration', 'Note', 'Artifact', 'UseCase') ";

            // SQL文 を作成
            strSQL = "select " + strFields +
                " from t_object  " +
                strWhere +
                " ORDER BY  ea_guid ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            OleDbDataReader reader = dbCom.ExecuteReader();
            List<ElementVO> retElements = new List<ElementVO>();

            // 結果を表示します。
            while (reader.Read())
            {
                ElementVO elem = new ElementVO();

                elem.elementId = DbUtil.readIntField(reader, 0);
                elem.guid = DbUtil.readStringField(reader, 1);
                elem.alias = StringUtil.excludeSpecialChar("t_object", "alias", elem.guid, DbUtil.readStringField(reader, 2));
                elem.author = DbUtil.readStringField(reader, 3);
                elem.created = (DateTime)reader.GetValue(4);
                elem.modified = (DateTime)reader.GetValue(5);
                elem.name = StringUtil.excludeSpecialChar("t_object", "name", elem.guid, DbUtil.readStringField(reader, 6));
                elem.notes = DbUtil.readStringField(reader, 7);
                elem.objectType = DbUtil.readIntField(reader, 8).ToString();
                //elem.packageId = reader.GetValue(9);
                elem.parentID = DbUtil.readIntField(reader,10);
                elem.stereoType = DbUtil.readStringField(reader, 11);
                elem.tag = DbUtil.readStringField(reader, 12);
                elem.treePos = DbUtil.readIntField(reader, 13);
                elem.eaType = DbUtil.readStringField(reader, 14);
                elem.version = DbUtil.readStringField(reader, 15);
                elem.visibility = DbUtil.readStringField(reader, 16);

                elem.genType = DbUtil.readStringField(reader, 17);
                elem.genFile = DbUtil.readStringField(reader, 18);

                // elem.Diagrams = null;
                // elem.Elements = null;

                // 要素の接続情報出力フラグ＝Trueの時のみデータ取得処理呼び出し
                // If EXP_ELEMENT_CONNECTOR_FLG = True Then
                // 	Set elem.Connectors = getEA_ConnectorsByElement(objConn, elem.ElementID)
                // Else
                // 	Set elem.Connectors = Nothing
                // End If

                if (g_AllAttributesInElementMap.ContainsKey(elem.elementId) == true)
                {
                    elem.attributes = g_AllAttributesInElementMap[elem.elementId];
                }
                else
                {
                    elem.attributes = new List<AttributeVO>();
                }

                if (g_AllMethodsInElementMap.ContainsKey(elem.elementId) == true)
                {
                    elem.methods = g_AllMethodsInElementMap[elem.elementId];
                }
                else
                {
                    elem.methods = new List<MethodVO>();
                }

                readTaggedValuesByElement(elem);

                // 返却用の要素のリストに追加
                retElements.Add(elem);
            }
            reader.Close();

            package.elements = retElements;
        }

        /// <summary>
        /// 要素のタグ付き値を取得し、taggedValues プロパティにセットする
        /// </summary>
        /// <param name="elem">対象の要素オブジェクト</param>
        private void readTaggedValuesByElement(ElementVO elem)
        {
            string strSQL;
            strSQL = "select PropertyID, ea_guid, Property, Notes, Value " +
                " from t_objectproperties " +
                " where object_id = " + elem.elementId +
                " order by ea_guid, Property, Value, Notes ";

            // SQLを実行する
            OleDbCommand dbCom = new OleDbCommand();
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            OleDbDataReader reader = dbCom.ExecuteReader();
            List<TaggedValueVO> retTagVals = new List<TaggedValueVO>();

            // 結果を表示します。
            while (reader.Read())
            {
                TaggedValueVO tag = new TaggedValueVO();
                // tag.propertyId = reader.GetValue(0);
                tag.guid = DbUtil.readStringField(reader, 1);
                tag.name = StringUtil.excludeSpecialChar("t_objectproperties", "name", tag.guid, DbUtil.readStringField(reader, 2));
                tag.notes = DbUtil.readStringField(reader, 3);
                tag.tagValue = StringUtil.excludeSpecialChar("t_objectproperties", "value", tag.guid, DbUtil.readStringField(reader, 4));

                retTagVals.Add(tag);
            }
            reader.Close();

            elem.taggedValues = retTagVals;
        }


        private void readDiagrams(PackageVO package)
        {
            // Console.WriteLine("readDiagrams(): packageID =" + package.packageId);

            string strSQL, strFields, strWhere;

            // 読み込む項目リスト
            strFields =
                "Diagram_ID, Package_ID, Diagram_Type, Name, Author, ShowDetails, " +
                "Notes, Stereotype, AttPub, AttPri, AttPro, Orientation, cx, cy, " +
                "Scale, CreatedDate, ModifiedDate, ShowForeign, ShowBorder, " +
                "ShowPackageContents, ea_guid, TPos, Swimlanes, StyleEx " ;

            // WHERE文: DBに対しては主に パッケージID で検索（キー＝IDで結合する必要があるため）
            strWhere = " where  Package_ID = " + package.packageId ;

            // SQL文 を作成
            strSQL = "select " + strFields +
                " from t_diagram  " +
                strWhere +
                " ORDER BY  TPos, Name ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            OleDbDataReader reader = dbCom.ExecuteReader();
            List<DiagramVO> retDiagrams = new List<DiagramVO>();

            // 結果を表示します。
            while (reader.Read())
            {
                DiagramVO diag = new DiagramVO();

                diag.changed = ' ';
                diag.diagramId = DbUtil.readIntField(reader, 0);
                diag.packageId = DbUtil.readIntField(reader, 1);
                // diag.parentId  = readIntField(reader, 2);
                diag.diagramType = DbUtil.readStringField(reader, 2);
                diag.name =  StringUtil.excludeSpecialChar("t_diagram", "name", diag.guid, DbUtil.readStringField(reader, 3));
                // diag.version = (DateTime)reader.GetValue(4);
                diag.author = DbUtil.readStringField(reader, 4);
                diag.showDetails = DbUtil.readIntField(reader, 5);
                diag.notes = DbUtil.readStringField(reader, 6);
                diag.stereotype = DbUtil.readStringField(reader, 7);
                diag.attPub = DbUtil.readBoolField(reader, 8);
                diag.attPri = DbUtil.readBoolField(reader, 9);
                diag.attPro = DbUtil.readBoolField(reader, 10);
                diag.orientation = DbUtil.readStringField(reader, 11);
                diag.cx = DbUtil.readIntField(reader, 12);
                diag.cy = DbUtil.readIntField(reader, 13);
                diag.scale = DbUtil.readIntField(reader, 14);
                diag.createdDate = DbUtil.readDateTimeField(reader, 15);
                diag.modifiedDate = (DateTime)reader.GetValue(16);
                diag.showForeign = DbUtil.readBoolField(reader, 17);
                diag.showBorder = DbUtil.readBoolField(reader, 18);
                diag.showPackageContents = DbUtil.readBoolField(reader, 19);
                diag.guid = DbUtil.readStringField(reader, 20);
                diag.treePos = DbUtil.readIntField(reader, 21);
                diag.swimlanes = DbUtil.readStringField(reader, 22);
                diag.styleEx = DbUtil.readStringField(reader, 23);

                diag.diagramObjects = null;
                diag.diagramLinks = null;

                readDiagramObjectsByDiagram(diag);

                readDiagramLinksByDiagram(diag);

                // 返却用の要素のリストに追加
                retDiagrams.Add(diag);
            }
            reader.Close();

            package.diagrams = retDiagrams;

        }


        private void readDiagramObjectsByDiagram(DiagramVO diag)
        {
            // Console.WriteLine("readDiagramObjectsByDiagram(): diagramId =" + diag.diagramId);

            string strSQL, strFields, strWhere;

            // 読み込む項目リスト
            strFields =
                "Diagram_ID, Object_ID, RectTop, RectLeft, " +
                "RectRight, RectBottom, Sequence, ObjectStyle, Instance_ID ";

            // WHERE文: DBに対しては主に パッケージID で検索（キー＝IDで結合する必要があるため）
            strWhere = " where  Diagram_ID = " + diag.diagramId;

            // SQL文 を作成
            strSQL = "select " + strFields +
                " from t_diagramobjects  " +
                strWhere +
                " ORDER BY  sequence ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            OleDbDataReader reader = dbCom.ExecuteReader();
            List<DiagramObjectVO> retDiagramObjs = new List<DiagramObjectVO>();

            // 結果を表示します。
            while (reader.Read())
            {
                DiagramObjectVO diagObj = new DiagramObjectVO();

                diagObj.changed = ' ';
                diagObj.diagramId = DbUtil.readIntField(reader, 0);
                diagObj.objectId = DbUtil.readIntField(reader, 1);
                diagObj.rectTop = DbUtil.readIntField(reader, 2);
                diagObj.rectLeft = DbUtil.readIntField(reader, 3);
                diagObj.rectRight = DbUtil.readIntField(reader, 4);
                diagObj.rectBottom = DbUtil.readIntField(reader, 5);
                diagObj.sequence = DbUtil.readIntField(reader, 6);
                diagObj.objectStyle = DbUtil.readStringField(reader, 7);
                diagObj.instanceId = DbUtil.readIntField(reader, 8);


                // 返却用の要素のリストに追加
                retDiagramObjs.Add(diagObj);
            }
            reader.Close();

            diag.diagramObjects = retDiagramObjs;
        }


        private void readDiagramLinksByDiagram(DiagramVO diag)
        {
            // Console.WriteLine("readDiagramLinksByDiagram(): diagramId =" + diag.diagramId);

            string strSQL, strFields, strWhere;

            // 読み込む項目リスト
            strFields =
                "DiagramID, ConnectorID, Geometry, Style, Hidden, " +
                "Path, Instance_ID ";

            // WHERE文: DBに対しては主に パッケージID で検索（キー＝IDで結合する必要があるため）
            strWhere = " where  DiagramID = " + diag.diagramId;

            // SQL文 を作成
            strSQL = "select " + strFields +
                " from t_diagramlinks  " +
                strWhere +
                " ORDER BY Instance_ID ";

            OleDbCommand dbCom = new OleDbCommand();

            // SQLを実行する
            dbCom.CommandText = strSQL;
            dbCom.Connection = objConn;

            OleDbDataReader reader = dbCom.ExecuteReader();
            List<DiagramLinkVO> retDiagramLinks = new List<DiagramLinkVO>();

            // 結果を表示します。
            while (reader.Read())
            {
                DiagramLinkVO diagLink = new DiagramLinkVO();

                diagLink.changed = ' ';
                diagLink.diagramId = DbUtil.readIntField(reader, 0);
                diagLink.connectorId = DbUtil.readIntField(reader, 1);
                diagLink.geometry = DbUtil.readStringField(reader, 2);
                diagLink.style = DbUtil.readStringField(reader, 3);
                diagLink.hidden = DbUtil.readBoolField(reader, 4);
                diagLink.path = DbUtil.readStringField(reader, 5);
                diagLink.instanceId = DbUtil.readIntField(reader, 6);

                // 返却用の要素のリストに追加
                retDiagramLinks.Add(diagLink);
            }
            reader.Close();

            diag.diagramLinks = retDiagramLinks;
        }

        #endregion

    }
}
