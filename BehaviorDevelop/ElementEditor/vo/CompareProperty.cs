namespace ElementEditor.vo
{
    /// <summary>
    /// 属性の差分結果
    /// </summary>
    public class CompareProperty
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// モデルデータ
        /// </summary>
        public string model { get; set; }

        /// <summary>
        /// ベースラインデータ
        /// </summary>
        public string baseline { get; set; }

        /// <summary>
        /// 変更状態
        /// </summary>
        public string status { get; set; }
    }
}
