using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace BehaviorDevelop.usercontrol
{
    /// <summary>
    /// ティップスリストボックス
    /// </summary>
    public class TipsListBox : ListBox
    {
        #region "定数"
        /// <summary>
        /// ティップスした文字列の後ろの文字列
        /// </summary>
        private string METHOD_AFTER_WORLD = " ";
        #endregion

        #region "変数"
        /// <summary>
        /// 接頭辞
        /// </summary>
        private string _prefix = "";
        public string Prefix { get { return _prefix; } set { _prefix = value; } }
        #endregion

        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TipsListBox()
            : base()
        {
            this.MinimumSize = new Size(200, 80);
        }
        #endregion

        #region "OnDrawItemイベント"
        /// <summary>
        /// OnDrawItemイベント
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            // セレクタフラグ
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? true : false;

            e.DrawBackground();

            Assembly asm = Assembly.GetExecutingAssembly();

            if (selected)
            {
                Image img = Image.FromStream(asm.GetManifestResourceStream("MM.App.Resources.focus.gif"));
                Brush b = new TextureBrush(img);
                e.Graphics.FillRectangle(b, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                Image img = Image.FromStream(asm.GetManifestResourceStream("MM.App.Resources.line.bmp"));
                Brush b = new TextureBrush(img);
                e.Graphics.FillRectangle(b, e.Bounds.X, e.Bounds.Y + 23, e.Bounds.Width, 1);
            }

            e.Graphics.DrawString(this.Items[e.Index].ToString(), this.Font, Brushes.Black, e.Bounds.X + 15, e.Bounds.Y + 6, StringFormat.GenericDefault);

            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                                        Color.Beige, 2, ButtonBorderStyle.Solid,
                                        Color.Beige, 2, ButtonBorderStyle.Solid,
                                        Color.Beige, 2, ButtonBorderStyle.Solid,
                                        Color.Beige, 2, ButtonBorderStyle.Solid);

            base.OnDrawItem(e);
        }
        #endregion

        #region "クリックイベント"
        /// <summary>
        /// クリックイベント
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnClick(EventArgs e)
        {
            if (this.Parent is RichTextBox)
            {
                if (this.SelectedIndex > -1)
                {
                    RichTextBox rtb = (RichTextBox)this.Parent;

                    string tips = this.SelectedItem.ToString();
                    tips = tips.Substring(this.Prefix.Length, tips.Length - this.Prefix.Length);

                    rtb.SelectionStart = rtb.SelectionStart + rtb.SelectionLength;
                    rtb.SelectedText = tips + this.METHOD_AFTER_WORLD;

                    this.Hide();
                    rtb.Controls.Remove(this);
                    this.Dispose();
                    rtb.Focus();
                }
            }
        }
        #endregion

        #region "キーダウンイベント"
        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.Parent is RichTextBox)
            {
                RichTextBox rtb = (RichTextBox)this.Parent;
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.SelectedIndex > -1)
                    {
                        string tips = this.SelectedItem.ToString();
                        tips = tips.Substring(this.Prefix.Length, tips.Length - this.Prefix.Length);

                        rtb.SelectionStart = rtb.SelectionStart + rtb.SelectionLength;
                        rtb.SelectedText = tips + this.METHOD_AFTER_WORLD;

                        this.Hide();
                        rtb.Controls.Remove(this);
                        this.Dispose();
                        rtb.Focus();
                    }
                }
                else if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
                {
                    rtb.Focus();
                }
            }
        }
        #endregion
    }
}
