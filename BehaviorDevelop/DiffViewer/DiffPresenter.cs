using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtifactFileAccessor.reader;
using ArtifactFileAccessor.util;
using ArtifactFileAccessor.vo;
using EA;
using System.IO;

namespace DiffViewer
{
    class DiffPresenter
    {
        /// <summary>
        /// 差異が検出された２つの属性の不一致な項目＝値をつなげた文字列を作成
        /// </summary>
        /// <param name="leftAttr">(in)左の属性VO</param>
        /// <param name="rightAttr">(in)右の属性VO</param>
        /// <param name="leftText">(out)左用の出力テキスト</param>
        /// <param name="rightText">(out)右用の出力テキスト</param>
        /// <returns></returns>
        internal static void getDisagreedAttributeDesc(AttributeVO leftAttr, AttributeVO rightAttr, ref string leftText, ref string rightText)
        {

            System.Text.StringBuilder lsb = new System.Text.StringBuilder();
            System.Text.StringBuilder rsb = new System.Text.StringBuilder();

            lsb.Append(leftAttr.name + "[" + leftAttr.alias + "]" + "\r\n");
            rsb.Append(rightAttr.name + "[" + rightAttr.alias + "]" + "\r\n");

            lsb.Append(leftAttr.guid + "\r\n");
            rsb.Append(rightAttr.guid + "\r\n");

            lsb.Append(leftAttr.getComparedString(rightAttr));
            rsb.Append(rightAttr.getComparedString(leftAttr));

            leftText = lsb.ToString();
            rightText = rsb.ToString();
            return;
        }


        /// <summary>
        /// 片方の属性のダンプ
        /// </summary>
        /// <param name="attr"></param>
        internal static void getMonoAttributeDesc(AttributeVO attr, ref string text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(attr.name + "[" + attr.alias + "]" + "\r\n");
            sb.Append(attr.guid + "\r\n");
            sb.Append(attr.getComparableString());

            text = sb.ToString();

            return;
        }

        /// <summary>
        /// 差異が検出された２つの操作の不一致な項目＝値をつなげた文字列を作成
        /// </summary>
        /// <param name="leftMth">(in)左の操作VO</param>
        /// <param name="rightMth">(in)右の操作VO</param>
        /// <param name="leftText">(out)左用の出力テキスト</param>
        /// <param name="rightText">(out)右用の出力テキスト</param>
        /// <returns></returns>
        internal static void getDisagreedMethodDesc(MethodVO leftMth, MethodVO rightMth, ref string leftText, ref string rightText)
        {

            System.Text.StringBuilder lsb = new System.Text.StringBuilder();
            System.Text.StringBuilder rsb = new System.Text.StringBuilder();

            lsb.Append(leftMth.name + "[" + leftMth.alias + "]" + "\r\n");
            rsb.Append(rightMth.name + "[" + rightMth.alias + "]" + "\r\n");

            lsb.Append(leftMth.guid + "\r\n");
            rsb.Append(rightMth.guid + "\r\n");

            if (!compareNullable(leftMth.stereoType, rightMth.stereoType))
            {
                lsb.Append("stereoType=" + leftMth.stereoType + "\r\n");
                rsb.Append("stereoType=" + rightMth.stereoType + "\r\n");
            }

            if (!compareNullable(leftMth.returnType, rightMth.returnType))
            {
                lsb.Append("returnType=" + leftMth.returnType + "\r\n");
                rsb.Append("returnType=" + rightMth.returnType + "\r\n");
            }

            if (!compareNullable(leftMth.visibility, rightMth.visibility))
            {
                lsb.Append("visibility=" + leftMth.visibility + "\r\n");
                rsb.Append("visibility=" + rightMth.visibility + "\r\n");
            }

            if (leftMth.pos != rightMth.pos)
            {
                lsb.Append("pos=" + leftMth.pos + "\r\n");
                rsb.Append("pos=" + rightMth.pos + "\r\n");
            }

            if (!compareNullable(leftMth.notes, rightMth.notes))
            {
                lsb.Append("[notes]\r\n" + leftMth.notes + "\r\n");
                rsb.Append("[notes]\r\n" + rightMth.notes + "\r\n");
            }

            if (!compareNullable(leftMth.behavior, rightMth.behavior))
            {
                lsb.Append("[behavior]\r\n" + leftMth.behavior);
                rsb.Append("[behavior]\r\n" + rightMth.behavior);
            }

            leftText = lsb.ToString();
            rightText = rsb.ToString();

            return;
        }


        /// <summary>
        /// 片方の操作のダンプ
        /// </summary>
        /// <param name="leftAtr"></param>
        /// <param name="rightAtr"></param>
        /// <returns></returns>
        internal static void getMonoMethodDesc(MethodVO mth, ref string text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(mth.name + "[" + mth.alias + "]" + "\r\n");
            sb.Append(mth.guid + "\r\n");
            sb.Append(mth.getComparableString());

            text = sb.ToString();
            return;
        }


        /// <summary>
        /// 差異が検出された２つのタグ付き値の不一致な項目＝値をつなげた文字列を作成
        /// </summary>
        /// <param name="leftTgv">(in)左の属性VO</param>
        /// <param name="rightTgv">(in)右の属性VO</param>
        /// <param name="leftText">(out)左用の出力テキスト</param>
        /// <param name="rightText">(out)右用の出力テキスト</param>
        /// <returns></returns>
        internal static void getDisagreedTaggedValueDesc(TaggedValueVO leftTgv, TaggedValueVO rightTgv, ref string leftText, ref string rightText)
        {

            System.Text.StringBuilder lsb = new System.Text.StringBuilder();
            System.Text.StringBuilder rsb = new System.Text.StringBuilder();

            lsb.Append(leftTgv.name + ":\r\n");
            rsb.Append(rightTgv.name + ":\r\n");

            lsb.Append(leftTgv.getComparedString(rightTgv));
            rsb.Append(rightTgv.getComparedString(leftTgv));

            leftText = lsb.ToString();
            rightText = rsb.ToString();
            return;
        }


        /// <summary>
        /// 片方のタグ付き値のダンプ
        /// </summary>
        /// <param name="leftAtr"></param>
        /// <param name="rightAtr"></param>
        /// <returns></returns>
        internal static void getMonoTaggedValueDesc(TaggedValueVO tgv, ref string text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(tgv.name +":\r\n");
            sb.Append(tgv.getComparableString());

            text = sb.ToString();
            return;
        }



        /// <summary>
        /// EAのAttributeを上書きもしくは追加する
        /// </summary>
        internal static void updateEaAttributeObject(ElementVO myElement, AttributeVO selectedAttribute)
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Element elem = null;
            int tmp = -1;

            // EAのAPIを使って属性をGUIDより検索
            EA.Attribute attrObj = (EA.Attribute)repo.GetAttributeByGuid(selectedAttribute.guid);

            // 取得できなかったら（該当するGUIDの属性が存在しなかったら）
            if (attrObj == null)
            {
                // この属性を持っているはずの要素をGUIDより検索
                elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
                if (elem == null)
                {
                    return;
                }
                attrObj = (EA.Attribute)elem.Attributes.AddNew(selectedAttribute.name, selectedAttribute.eaType);
            }

            // 更新前後で更新ログ出力
            writeUpdateLogAttribute(attrObj, false);

            attrObj.Name = selectedAttribute.name;
            attrObj.AttributeGUID = selectedAttribute.guid;
            attrObj.Alias = selectedAttribute.alias;
            attrObj.StereotypeEx = selectedAttribute.stereoType;
            attrObj.Notes = selectedAttribute.notes;

            attrObj.AllowDuplicates = selectedAttribute.allowDuplicates;
            if ("".Equals(selectedAttribute.classifierID) || !Int32.TryParse(selectedAttribute.classifierID, out tmp))
            {
                selectedAttribute.classifierID = "0";
            }
            else
            {
                attrObj.ClassifierID = tmp;
            }

            attrObj.Container = selectedAttribute.container;
            attrObj.Containment = selectedAttribute.containment;
            attrObj.Default = selectedAttribute.defaultValue;
            attrObj.IsCollection = selectedAttribute.isCollection;
            attrObj.IsConst = selectedAttribute.isConst;
            attrObj.IsDerived = selectedAttribute.isDerived;
            // attr.IsID = selectedAttribute.;
            attrObj.IsOrdered = selectedAttribute.isOrdered;
            attrObj.IsStatic = selectedAttribute.isStatic;
            attrObj.Length = selectedAttribute.length.ToString();
            attrObj.LowerBound = selectedAttribute.lowerBound.ToString();
            attrObj.Precision = selectedAttribute.precision.ToString();
            attrObj.Pos = selectedAttribute.pos;
            // attr.RedefinedProperty = selectedAttribute.;
            attrObj.Scale = selectedAttribute.scale.ToString();
            attrObj.Stereotype = selectedAttribute.stereoType;
            // attr.Style = selectedAttribute.;
            // attr.SubsettedProperty = selectedAttribute.;
            attrObj.StyleEx = selectedAttribute.styleEx;
            attrObj.Type = selectedAttribute.eaType;
            attrObj.UpperBound = selectedAttribute.upperBound.ToString();
            attrObj.Visibility = selectedAttribute.visibility;

            attrObj.Update();
            //						elem.Update();

            // 更新前後で更新ログ出力
            writeUpdateLogAttribute(attrObj, true);
        }


        /// <summary>
        /// 属性の更新前、更新後の情報をログファイルに出力
        /// </summary>
        /// <param name="attr">対象のEA.Attributeオブジェクト</param>
        /// <param name="afterUpdateFlag">更新前／更新後を示す(true=後)</param>
        private static void writeUpdateLogAttribute(EA.Attribute attr, bool afterUpdateFlag)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@"C:\ea-artifacts\DiffViewerUpdate.log", true);

                if (afterUpdateFlag)
                {
                    sw.WriteLine("■属性の書き込みが完了しました(ID={0}, GUID={1})。", attr.AttributeID, attr.AttributeGUID);
                }
                else
                {
                    sw.WriteLine("〇属性の更新前情報を表示します(ID={0}, GUID={1})。", attr.AttributeID, attr.AttributeGUID);
                }

                sw.WriteLine("  AttributeID = " + attr.AttributeID);
                sw.WriteLine("  AttributeGUID = " + attr.AttributeGUID);
                sw.WriteLine("  Name = " + attr.Name);
                sw.WriteLine("  Alias = " + attr.Alias);
                sw.WriteLine("  StereotypeEx = " + attr.StereotypeEx);
                sw.WriteLine("  Notes = " + attr.Notes);
                sw.WriteLine("  AllowDuplicate = " + attr.AllowDuplicates);
                sw.WriteLine("  ClassifierID = " + attr.ClassifierID);
                sw.WriteLine("  Container = " + attr.Container);
                sw.WriteLine("  Containment = " + attr.Containment);
                sw.WriteLine("  Default = " + attr.Default);
                sw.WriteLine("  IsCollection = " + attr.IsCollection);
                sw.WriteLine("  IsConst = " + attr.IsConst);
                sw.WriteLine("  IsDerived = " + attr.IsDerived);
                sw.WriteLine("  IsOrdered = " + attr.IsOrdered);
                sw.WriteLine("  IsStatic = " + attr.IsStatic);
                sw.WriteLine("  Length = " + attr.Length);
                sw.WriteLine("  LowerBound = " + attr.LowerBound);
                sw.WriteLine("  Precision = " + attr.Precision);
                sw.WriteLine("  Pos = " + attr.Pos);
                sw.WriteLine("  Scale = " + attr.Scale);
                sw.WriteLine("  Stereotype = " + attr.Stereotype);
                sw.WriteLine("  StyleEx = " + attr.StyleEx);
                sw.WriteLine("  Type = " + attr.Type);
                sw.WriteLine("  UpperBound = " + attr.UpperBound);
                sw.WriteLine("  Visibility = " + attr.Visibility);
                sw.WriteLine("");

                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        /// <summary>
        /// GUIDを指定してEA上の属性オブジェクトを取得する
        /// </summary>
        /// <param name="attributeGuid">検索対象属性のGUID</param>
        /// <returns>合致するGUIDでヒットした属性オブジェクト。ヒットしなかったらnull</returns>
        private static EA.Attribute getAttributeByGuid(string attributeGuid)
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Attribute attrObj = (EA.Attribute)repo.GetAttributeByGuid(attributeGuid);
            if (attrObj != null)
            {
                return attrObj;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// EAのMethod（t_operator）を上書きもしくは追加する
        /// </summary>
        internal static void updateEaMethodObject(ElementVO myElement, MethodVO selectedMethod)
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Element elem = null;

            // EA.Repository の GetMethodByGuid を呼んでEA上の該当メソッドオブジェクトを取得する
            EA.Method mthObj = getMethodByGuid(selectedMethod.guid);

            if (mthObj == null)
            {
                elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
                if (elem == null)
                {
                    return;
                }

                mthObj = (EA.Method)elem.Methods.AddNew(selectedMethod.name, selectedMethod.returnType);
            }

            writeUpdateLogMethod(mthObj, false);

            mthObj.Name = selectedMethod.name;
            mthObj.MethodGUID = selectedMethod.guid;
            mthObj.Alias = selectedMethod.alias;
            mthObj.StereotypeEx = selectedMethod.stereoType;
            mthObj.Notes = selectedMethod.notes;
            mthObj.Behavior = selectedMethod.behavior;

            mthObj.Abstract = selectedMethod.isAbstract;
            mthObj.ClassifierID = selectedMethod.classifierID;
            mthObj.Code = selectedMethod.code;
            mthObj.Concurrency = selectedMethod.concurrency;
            mthObj.IsConst = selectedMethod.isConst;
            mthObj.IsLeaf = selectedMethod.isLeaf;
            mthObj.IsPure = selectedMethod.isPure;
            mthObj.IsQuery = selectedMethod.isQuery;
            mthObj.IsRoot = selectedMethod.isRoot;
            mthObj.IsStatic = selectedMethod.isStatic;
            // mth.IsSynchronized = selectedMethod;
            mthObj.Pos = selectedMethod.pos;
            mthObj.ReturnIsArray = selectedMethod.returnIsArray;
            mthObj.ReturnType = selectedMethod.returnType;
            mthObj.StateFlags = selectedMethod.stateFlags;
            mthObj.StyleEx = selectedMethod.styleEx;
            mthObj.Throws = selectedMethod.throws;
            mthObj.Visibility = selectedMethod.visibility;
            mthObj.Update();

            // 既にパラメータが設定されている場合は一旦削除
            for (short i = 0; i < mthObj.Parameters.Count; i++)
            {
                mthObj.Parameters.Delete(i);
            }

            // XMLから読み込まれたパラメータの値を設定する
            foreach (ParameterVO prm in selectedMethod.parameters)
            {
                EA.Parameter paramObj = (EA.Parameter)mthObj.Parameters.AddNew(prm.name, prm.eaType);
                paramObj.Alias = prm.alias;
                paramObj.ClassifierID = prm.classifierID;
                paramObj.Default = prm.defaultValue;
                paramObj.IsConst = prm.isConst;
                paramObj.Kind = prm.kind;
                paramObj.Name = prm.name;
                paramObj.Notes = prm.notes;
                paramObj.ParameterGUID = prm.guid;
                paramObj.Position = prm.pos;
                paramObj.StereotypeEx = prm.stereoType;
                // paramObj.Style = prm.Style ;
                // paramObj.StyleEx = prm.StyleEx ;
                paramObj.Type = prm.eaType;
                paramObj.Update();
            }

            mthObj.Update();

            writeUpdateLogMethod(mthObj, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mth"></param>
        /// <param name="afterUpdateFlag"></param>
        private static void writeUpdateLogMethod(EA.Method mth, bool afterUpdateFlag)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@"C:\DesignHistory\DiffViewerUpdate.log", true);


                if (afterUpdateFlag)
                {
                    sw.WriteLine("■操作の書き込みが完了しました(ID={0}, GUID={1})。", mth.MethodID, mth.MethodGUID);
                }
                else
                {
                    sw.WriteLine("〇操作の更新前情報を表示します(ID={0}, GUID={1})。", mth.MethodID, mth.MethodGUID);
                }

                sw.WriteLine("  MethodID =" + mth.MethodID);
                sw.WriteLine("  MethodGUID =" + mth.MethodGUID);
                sw.WriteLine("  Name =" + mth.Name);
                sw.WriteLine("  Alias =" + mth.Alias);
                sw.WriteLine("  StereotypeEx =" + mth.StereotypeEx);
                sw.WriteLine("  Notes =" + mth.Notes);
                sw.WriteLine("  Behavior =" + mth.Behavior);
                sw.WriteLine("  Abstract =" + mth.Abstract);
                sw.WriteLine("  ClassifierID =" + mth.ClassifierID);
                sw.WriteLine("  Code =" + mth.Code);
                sw.WriteLine("  Concurrency =" + mth.Concurrency);
                sw.WriteLine("  IsConst =" + mth.IsConst);
                sw.WriteLine("  IsLeaf =" + mth.IsLeaf);
                sw.WriteLine("  IsPure =" + mth.IsPure);
                sw.WriteLine("  IsQuery =" + mth.IsQuery);
                sw.WriteLine("  IsRoot =" + mth.IsRoot);
                sw.WriteLine("  IsStatic =" + mth.IsStatic);
                sw.WriteLine("  Pos =" + mth.Pos);
                sw.WriteLine("  ReturnIsArray =" + mth.ReturnIsArray);
                sw.WriteLine("  ReturnType =" + mth.ReturnType);
                sw.WriteLine("  StateFlags =" + mth.StateFlags);
                sw.WriteLine("  StyleEx =" + mth.StyleEx);
                sw.WriteLine("  Throws =" + mth.Throws);
                sw.WriteLine("  Visibility =" + mth.Visibility);

                sw.WriteLine("");

                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

        }

        /// <summary>
        /// GUIDを指定してEA上のメソッドオブジェクトを取得する
        /// </summary>
        /// <param name="methodGuid">検索対象メソッドのGUID</param>
        /// <returns>合致するGUIDでヒットしたメソッドオブジェクト。ヒットしなかったらnull</returns>
        private static EA.Method getMethodByGuid(string methodGuid)
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Method mthObj = (EA.Method)repo.GetMethodByGuid(methodGuid);
            return mthObj;
        }


        /// <summary>
        /// EAのTaggedValueを上書きもしくは追加する
        /// </summary>
        internal static void updateEaTaggedValueObject(ElementVO myElement, TaggedValueVO selectedTag)
        {
            EA.Repository repo = ProjectSetting.getVO().eaRepo;
            EA.Element elem = null;

            // この属性を持っているはずの要素をGUIDより検索
            elem = (EA.Element)repo.GetElementByGuid(myElement.guid);
            if (elem == null)
            {
                return;
            }

            TaggedValue tvObj = null;
            if (elem.TaggedValues != null && elem.TaggedValues.Count > 0)
            {

                for (int i = 0; i < elem.TaggedValues.Count; i++)
                {
                    tvObj = elem.TaggedValues.GetAt((short)i);
                    if (selectedTag.guid == tvObj.PropertyGUID)
                    {
                        break;
                    }
                }
            }

            // 結果的に TaggedValue のオブジェクトが取得できなかったら、この要素の配下にタグ付き値を追加する
            if (tvObj == null)
            {
                tvObj = elem.TaggedValues.AddNew(selectedTag.name, "");
            }

            // 更新前後で更新ログ出力
            writeUpdateLogTaggedValue(tvObj, false);

            tvObj.Name = selectedTag.name;
            tvObj.Notes = selectedTag.notes;
            // tvObj.ObjectType = selectedTag.name;
            tvObj.PropertyGUID = selectedTag.guid;
            tvObj.Value = selectedTag.tagValue;

            // トランザクションのコミット
            tvObj.Update();

            // 更新前後で更新ログ出力
            writeUpdateLogTaggedValue(tvObj, true);
        }

        /// <summary>
        /// タグ付き値の更新前、更新後の情報をログファイルに出力
        /// </summary>
        /// <param name="tgv">対象のEA.TaggedValueオブジェクト</param>
        /// <param name="afterUpdateFlag">更新前／更新後を示す(true=後)</param>
        private static void writeUpdateLogTaggedValue(EA.TaggedValue tgv, bool afterUpdateFlag)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@"C:\ea-artifacts\DiffViewerUpdate.log", true);

                if (afterUpdateFlag)
                {
                    sw.WriteLine("■タグ付き値の書き込みが完了しました(ID={0}, GUID={1})。", tgv.PropertyID, tgv.PropertyGUID);
                }
                else
                {
                    sw.WriteLine("〇タグ付き値の更新前情報を表示します(ID={0}, GUID={1})。", tgv.PropertyID, tgv.PropertyGUID);
                }

                sw.WriteLine("  PropertyID = " + tgv.PropertyID);
                sw.WriteLine("  PropertyGUID = " + tgv.PropertyGUID);
                sw.WriteLine("  Name = " + tgv.Name);
                sw.WriteLine("  Notes = " + tgv.Notes);
                sw.WriteLine("  Value = " + tgv.Value);
                sw.WriteLine("");

                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }






        /// <summary>
        /// Nullの場合でも比較可能なstringの比較メソッド
        /// </summary>
        /// <param name="l">左の文字列</param>
        /// <param name="r">右の文字列</param>
        /// <returns>一致したならtrue,一致しないならfalse</returns>
        private static Boolean compareNullable(string l, string r)
        {
            // 左が null の場合
            if (l == null)
            {
                // 右も null なら true
                if (r == null)
                {
                    return true;
                }
                else
                {
                    // 右が not null なら false
                    return false;
                }
            }
            else
            {
                // 左が not null の場合

                // 右が null なら false
                if (r == null)
                {
                    return false;
                }
                else
                {
                    // 両方 not null なので、string#Equalsの結果を返却
                    return l.Equals(r);
                }
            }

        }

    }
}
