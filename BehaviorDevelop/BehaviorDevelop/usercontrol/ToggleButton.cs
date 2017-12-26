using System;
using System.Windows.Forms;

namespace BehaviorDevelop.usercontrol
{
    /// <summary>
    /// トグルボタン
    /// </summary>
    public class ToggleButton : CheckBox
    {
        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ToggleButton()
            : base()
        {
            base.Appearance = System.Windows.Forms.Appearance.Button;
            base.AutoSize = true;
            base.Image = global::BehaviorDevelop.Properties.Resources.NumberingOFF;
        }
        #endregion

        #region "チェックプロパティの変更イベント"
        /// <summary>
        /// チェックプロパティが変更される時に発生する
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCheckedChanged(EventArgs e)
        {
            if (base.Checked)
            {
                base.Image = global::BehaviorDevelop.Properties.Resources.NumberingON;
            }
            else
            {
                base.Image = global::BehaviorDevelop.Properties.Resources.NumberingOFF;
            }

            base.OnCheckedChanged(e);
        }
        #endregion
    }
}