using System;
using System.Collections.Generic;

using ArtifactFileAccessor.vo;
using System.IO;

namespace ArtifactFileAccessor.util
{
	/// <summary>
	/// Description of ElementDiffer.
	/// </summary>
	public class ElementDiffer
	{
		public Boolean skipElementTaggedValue = true ;
		public Boolean skipElementTPosFlg = false ;
		public Boolean skipAttributeNoteFlg = false ;
		public Boolean skipMethodNoteFlg = false ;
		public Boolean skipAttributePosFlg = false ;
		public Boolean skipMethodPosFlg = false ;

		public ElementDiffer()
		{
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="leftElm"></param>
		/// <param name="rightElm"></param>
		/// <param name="outputDetailFileFlg"></param>
		/// <returns></returns>
		public static ElementVO getMergedElement(ElementVO leftElm, ElementVO rightElm) {
			ElementDiffer differ = new ElementDiffer();
			differ.skipAttributeNoteFlg = false;
			differ.skipElementTaggedValue = false;
			differ.skipElementTPosFlg = true;
			differ.skipAttributeNoteFlg = false;
			differ.skipAttributePosFlg = false;
			differ.skipMethodNoteFlg = false;
			differ.skipMethodPosFlg = false;

            return differ.getDisagreedElement(leftElm, rightElm);
		}

		/// <summary>
		/// 要素の不一致部分の抽出
		/// </summary>
		/// <param name="leftElem">左の要素</param>
		/// <param name="rightElem">右の要素</param>
		/// <returns></returns>
		public ElementVO getDisagreedElement(ElementVO leftElm, ElementVO rightElm) {
            // 出力する要素のインスタンスは一旦左側要素のクローンとする
			ElementVO outElm = leftElm.Clone();
            outElm.changed = ' ';

            // 要素自体が保持するプロパティの比較
            ElementPropertyVO lProp = new ElementPropertyVO(leftElm);
            ElementPropertyVO rProp = new ElementPropertyVO(rightElm);

            if( lProp.getComparableString() != rProp.getComparableString())
            {
                outElm.changed = 'U';
                outElm.propertyChanged = 'U';
                outElm.srcElementProperty = lProp;
                outElm.destElementProperty = rProp;
            }


            leftElm.sortChildNodesGuid();
            rightElm.sortChildNodesGuid();


            // 要素が保持する属性リストの比較
            Int16 lCnt, rCnt;
            AttributeVO lAtr, rAtr, oAtr;
			List<AttributeVO> outAttributeList = new List<AttributeVO>();
			for (lCnt=0, rCnt=0; lCnt < leftElm.attributes.Count || rCnt < rightElm.attributes.Count; ) {
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
                        oAtr.srcAttribute = lAtr;
                        oAtr.destAttribute = rAtr;
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
			MethodVO lMth, rMth, oMth;
			List<MethodVO> outMethods = new List<MethodVO>();
			for (lCnt=0, rCnt=0; lCnt < leftElm.methods.Count || rCnt < rightElm.methods.Count; ) {
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
                        oMth.srcMethod = lMth;
                        oMth.destMethod = rMth;
						outMethods.Add(oMth);
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

            // 要素が保持するタグ付き値リストの比較
            TaggedValueVO lTag, rTag, oTag;
            List<TaggedValueVO> outTaggedValues = new List<TaggedValueVO>();
            for (lCnt = 0, rCnt = 0; lCnt < leftElm.taggedValues.Count || rCnt < rightElm.taggedValues.Count;)
            {
                // 左側が最終の操作に達した場合
                if (lCnt >= leftElm.taggedValues.Count)
                {
                    // 右側の操作が追加されたものとして出力パッケージリストに追加
                    oTag = rightElm.taggedValues[rCnt];
                    oTag.changed = 'C';
                    outTaggedValues.Add(oTag);
                    rCnt++;
                    continue;
                }

                // 右側が最終の操作に達した場合
                if (rCnt >= rightElm.taggedValues.Count)
                {
                    // 左側の操作が削除されたものとして出力パッケージリストに追加
                    oTag = leftElm.taggedValues[lCnt];
                    oTag.changed = 'D';
                    outTaggedValues.Add(oTag);
                    lCnt++;
                    continue;
                }


                lTag = leftElm.taggedValues[lCnt];
                rTag = rightElm.taggedValues[rCnt];

                // GUIDで一致するものがあった場合:
                if (compareTaggedValueGuid(lTag, rTag) == 0)
                {
                    oTag = getDisagreedTaggedValue(lTag, rTag);
                    if (oTag != null)
                    {
                        oTag.changed = 'U';
                        oTag.srcTaggedValue = lTag;
                        oTag.destTaggedValue = rTag;
                        outTaggedValues.Add(oTag);
                    }

                    lCnt++;
                    rCnt++;
                }
                else
                {
                    // GUIDで一致するものが無かった場合: L > R なら Rの追加、 R < L なら Lの削除 とみなす
                    if (compareTaggedValueGuid(lTag, rTag) > 0)
                    {
                        rTag.changed = 'C';

                        outTaggedValues.Add(rTag);
                        rCnt++;
                    }
                    else
                    {
                        lTag.changed = 'D';
                        outTaggedValues.Add(lTag);

                        lCnt++;
                    }
                }
            }
            outElm.taggedValues = outTaggedValues;

            if (outElm.attributes.Count == 0 && outElm.methods.Count == 0 && outElm.taggedValues.Count == 0) {
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

            // 属性のタグ付き値の比較
            if (compareTaggedValues(leftAtr.taggedValues, rightAtr.taggedValues) != 0)
            {
                outAtr.changed = 'U';
            }


            if ( outAtr.changed == ' ' ) {
				return null;
			} else {
				// 空白以外なら 'U' で何かしら変更があったとみなされるため、左を比較元、右を比較先として情報を残す
				outAtr.srcAttribute = leftAtr;
				outAtr.destAttribute = rightAtr;

				return outAtr ;
			}

		}

        /// <summary>
        /// 属性、操作、パラメータに付加されるタグ付き値のリストの内容比較
        /// （string#CompareTo()メソッドを利用しての比較なので、戻り値はintを返す）
        /// </summary>
        /// <param name="leftTaggedValues"></param>
        /// <param name="rightTaggedValues"></param>
        /// <returns></returns>
        private int compareTaggedValues(List<TaggedValueVO> leftTaggedValues, List <TaggedValueVO> rightTaggedValues)
        {
            // 左、右どちらか（どちらも）がnullの場合に落ちないように制御
            if( leftTaggedValues == null )
            {
                if( rightTaggedValues == null )
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (rightTaggedValues == null)
                {
                    return -1;
                }
            }

            // GUID順にソート
            leftTaggedValues.Sort(new TaggedValueGuidComparer());
            rightTaggedValues.Sort(new TaggedValueGuidComparer());

            // 左のリスト内容をテキストに出力
            StringWriter leftTaggedValuesText = new StringWriter();
            foreach (TaggedValueVO tv in leftTaggedValues)
            {
                leftTaggedValuesText.WriteLine(tv.ToString());
            }

            // 右のリスト内容をテキストに出力
            StringWriter rightTaggedValuesText = new StringWriter();
            foreach (TaggedValueVO tv in rightTaggedValues)
            {
                rightTaggedValuesText.WriteLine(tv.ToString());
            }

            // 左のテキストと右のテキストを
            return leftTaggedValuesText.ToString().CompareTo(rightTaggedValuesText.ToString());
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

			// メソッドのタグ付き値の比較
			if( compareTaggedValues(leftMth.taggedValues, rightMth.taggedValues) != 0 ) {
                outMth.changed = 'U';
            }

			// パラメータ以降（パラメータ、パラメータタグ）の比較
			if( compareParameters(leftMth.parameters, rightMth.parameters) != 0 ) {
                outMth.changed = 'U';
            }


            // changedの値が空白なら結果として差異がなかったため、nullを返す
            if ( outMth.changed == ' ' ) {
				return null;
			} else {
				// 空白以外なら 'U' で何かしら変更があったとみなされるため、左を比較元、右を比較先として情報を残す
				outMth.srcMethod = leftMth;
				outMth.destMethod = rightMth;
				return outMth ;
			}
		}

        /// <summary>
        /// 属性、操作、パラメータに付加されるタグ付き値
        /// </summary>
        /// <param name="leftTaggedValues"></param>
        /// <param name="rightTaggedValues"></param>
        /// <returns></returns>
        private int compareParameters(List<ParameterVO> leftParameters, List<ParameterVO> rightParameters)
        {
            // 左、右どちらか（どちらも）がnullの場合に落ちないように制御
            if (leftParameters == null)
            {
                if (rightParameters == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (rightParameters == null)
                {
                    return -1;
                }
            }

            // GUID順にソート
            leftParameters.Sort(new ParameterGuidComparer());
            rightParameters.Sort(new ParameterGuidComparer());

            // 左のリスト内容をテキストに出力
            StringWriter leftParametersText = new StringWriter();
            foreach (ParameterVO prm in leftParameters)
            {
                leftParametersText.WriteLine(prm.ToString());
            }

            // 右のリスト内容をテキストに出力
            StringWriter rightParametersText = new StringWriter();
            foreach (ParameterVO prm in rightParameters)
            {
                rightParametersText.WriteLine(prm.ToString());
            }

            return leftParametersText.ToString().CompareTo(rightParametersText.ToString());
        }





        /// <summary>
        /// 不一致なタグ付き値の抽出
        /// </summary>
        /// <param name="leftTag"></param>
        /// <param name="rightTag"></param>
        /// <returns></returns>
        private TaggedValueVO getDisagreedTaggedValue(TaggedValueVO leftTag, TaggedValueVO rightTag)
        {
            TaggedValueVO outTag;

            outTag = leftTag.Clone();
            outTag.changed = ' ';

            if (!compareNullable(leftTag.name, rightTag.name))
            {
                outTag.name = leftTag.name + " → " + rightTag.name;
                outTag.changed = 'U';
            }

            if (!compareNullable(leftTag.tagValue, rightTag.tagValue))
            {
                outTag.tagValue = leftTag.tagValue + " → " + rightTag.tagValue;
                outTag.changed = 'U';
            }

            if (!compareNullable(leftTag.notes, rightTag.notes))
            {
                outTag.notes = leftTag.notes + "\r\n------ ↓ ↓ ↓ ↓ ------\r\n" + rightTag.notes;
                outTag.changed = 'U';
            }

            if (outTag.changed == ' ')
            {
                return null;
            }
            else
            {
                // 空白以外なら 'U' で何かしら変更があったとみなされるため、左を比較元、右を比較先として情報を残す
                outTag.srcTaggedValue = leftTag;
                outTag.destTaggedValue = rightTag;

                return outTag;
            }

        }




        /// <summary>
        /// マージ時に使用：GUIDによる属性の比較
        /// </summary>
        /// <param name="leftAtr">左の属性</param>
        /// <param name="rightAtr">右の属性</param>
        /// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
        private static int compareAttributeGuid(AttributeVO leftAtr, AttributeVO rightAtr) {
			return leftAtr.guid.CompareTo(rightAtr.guid);
		}

		/// <summary>
		/// マージ時に使用：GUIDによる操作の比較
		/// </summary>
		/// <param name="leftAtr">左の操作</param>
		/// <param name="rightAtr">右の操作</param>
		/// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
		private static int compareMethodGuid(MethodVO leftMth, MethodVO rightMth) {
			return leftMth.guid.CompareTo(rightMth.guid);
		}


        /// <summary>
        /// マージ時に使用：GUIDによるタグ付き値の比較
        /// </summary>
        /// <param name="leftTag">左の操作</param>
        /// <param name="rightTag">右の操作</param>
        /// <returns>string#CompareTo()の戻り値と同じ（L=Rなら0, L&gt;Rなら1, L&lt;Rなら-1）</returns>
        private static int compareTaggedValueGuid(TaggedValueVO leftTag, TaggedValueVO rightTag)
        {
            return leftTag.guid.CompareTo(rightTag.guid);
        }


        private static Boolean compareNullable(string l, string r) {
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

    }
}
