
using System.IO;

namespace SearchColor
{
    internal class ColorFileManager
    {
        internal static void SaveColor(string newColorText)
        {
            // CSVファイルのパス
            string csvFilePath = "oldcolor.csv";

            // 現在の色リストを取得
            List<string> colors = new List<string>();
            if (File.Exists(csvFilePath))
            {
                colors.AddRange(File.ReadAllLines(csvFilePath));
            }

            // 新しい色が既存リストに存在するかチェック
            if (colors.Contains(newColorText))
            {
                // 重複する色が存在する場合、その色を削除
                colors.Remove(newColorText);
            }

            // 新しい色をリストの最後に追加
            colors.Add(newColorText);

            // リストが3つ以上なら最も古い色を削除
            if (colors.Count > 3)
            {
                colors.RemoveAt(0);
            }

            // CSVファイルにリストを保存
            File.WriteAllLines(csvFilePath, colors);
        }
    }
}