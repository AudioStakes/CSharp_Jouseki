using System;
using System.IO;
using System.Linq;

namespace CSharp_Jouseki
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    class Chapter8
    {
        // 8.5 日時の計算（応用）
        // 次の指定曜日を求める
        public static Date Time NextDay(DateTime date, DayOfWeek dayOfWeek)
        {
            var days = (int)dayOfWeek - (int)(date.DayOfWeek);
            if (days <= 0)
            {
                days += 7;
            }
            return date.AddDays(days);
        }

        // 年齢を求める
        public static int GetAge(DateTime birthday, DateTime targetDay)
        {
            var age = targetDay.Year - birthday.Year;
            // 誕生日前か判別
            if (targetDay < birthday.AddYears(age))
            {
                age--;
            }
            return age;
        }

        // 指定した日が第何週か求める
        public static int NthWeekk(DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var firstDayOfWeek = (int)(firstDay.DayOfWeek); // 日曜日が０で土曜日が６
            return (date.Day + firstDayOfWeek - 1) / 7 + 1;
        }

        // 指定した月の第n回目のX曜日の日付を求める
    }

    class Chapter9 // ファイルの操作
    {
        public static void Main()
        {
            // 9.1 テキストファイルの入力
            // テキストファイルを１行ずつ読み込む
            var filePath = @"C:¥Example¥Greeting.txt";
            if (File.Exists(filePath))
            {
                // usingでリソースの解放を確実に行う。以前はtry-finally構文が使われていた。
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        Console.WriteLine(line);
                    }
                }
            }

            // テキストファイルを一気に読み込む。小さいファイル専用の方法。
            var filePath = @"C:¥Example¥Greeting.txt";
            var lines = File.ReadAllLines(filePath, Encoding.UTF8); // 結果をstring[]で返す
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            // テキストファイルをIEnumerable<string>として扱う
            // .NET Framework 4以上の環境では、↑のReadALllLinesよりも↓のReadLinesメソッドを使うようにすること
            var filePath = @"C:¥Example¥Greeting.txt";
            lines = File.ReadLines(filePath, Encoding.UTF8); // 結果をIEnumerable<string>で返す
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            // ReadLinesメソッドとLINQの組み合わせ
            // 先頭の10行を取り出す
            lines = File.ReadLines(filePath, Encoding.UTF8)
                            .Take(10)
                            .ToArray();
            // 条件に一致した行数をカウントする
            lines = File.ReadLines(filePath, Encoding.UTF8)
                            .Count(s => s.Contains("Windows"))
            // 条件に一致した行だけ取り出す(↓空文字や空白行以外を取り出す)
            lines = File.ReadLines(filePath, Encoding.UTF8)
                            .Where(s => !String.IsNullOrWhiteSpace(s))
                            .ToArray();
            // 条件に一致した行が存在しているか調べる(↓数字だけから成る行が存在するか)
            var exists = File.ReadLines(filePath, Encoding.UTF8)
                            .Where(s => !String.IsNullOrEmpty(s)) // Allメソッドは空の行に対してtrueを返すので、それを取り除く
                             .Any(s => s.All(c => Char.IsDigit(c)));
            // 重複行を取り除き行の長さが長い順に並べ替える
            lines = File.ReadLines(filePath, Encoding.UTF8)
                        .Distinct()
                        .OrderBy(s => s.Length)
                        .ToArray();
            // 各行の先頭に行番号を付加する
            lines = File.ReadLines(filePath)
                    .Select((s, ix) => String.Format("{0,4}: {1}", ix + 1, s))
                    .ToArray();

            // 9.2 テキストファイルへの出力
            // テキストファイルに１行ずつ文字列を出力
            filePath = @"C:¥Example¥Greeting.txt"; //ファイルがなければ新規作成される
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("色はにほへど　散りぬるを");
                writer.WriteLine("我が世たれぞ　常ならむ");
            }

            // 既存テキストファイルの末尾に行を追加する
            lines = new[] { "====", "京の夢", "大阪の夢", };
            filePath = @"C:¥Example¥Greeting.txt"; //ファイルがなければ新規作成される
            using (var writer = new StreamWriter(filePath, append: true)) //名前付き引数でわかりやすく書く
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }

            // 文字列の配列を一気にファイルに出力する
            lines = new[] { "====", "京の夢", "大阪の夢", };
            filePath = @"C:¥Example¥Greeting.txt"; //ファイルがなければ新規作成される
            File.WriteAllLines(filePath, lines);

            // LINQクエリの結果をファイルに出力する
            var names = new List<string>{
                "====", "京の夢", "大阪の夢",
            };
            filePath = @"C:¥Example¥Greeting.txt"; //ファイルがなければ新規作成される
            File.WriteAllLines(filePath, lines.Where(s => s.Length > 5)); //文字列の長さが５文字より長いもの

            // 既存テキストファイルの先頭に行を挿入する
            filePath = @"C:¥Example¥Greeting.txt"; //ファイルがなければ新規作成される
            using (var stream = new FileStream(filePath, FileMode.Open,
                                               FileAccess.ReadWrite, FileShare.None))
            {
                using (var reader = new StreamReader(stream))
                using (var writer = new StreamWriter(stream))
                {
                    string texts = reader.ReadToEnd(); // すべての行が読み込まれている。改行コードもそのまま
                    stream.Position = 0; // ポジションをファイル先頭に戻す
                    writer.WriteLine("挿入する新しい行１");
                    writer.WriteLine("挿入する新しい行２");
                    writer.Write(texts);
                }
            }

            // 9.3 ファイルの操作
            // ファイルの有無を調べる
            //Fileクラスを使った場合
            if (File.Exists(@"C:¥Example¥Greeting.txt"))
            {
                Console.WriteLine("すでに存在しています");
            }

            //FileInfoクラスを使った場合
            var fi = new FileInfo(@"C:¥Example¥Greeting.txt");
            if (fi.Exists)
            {
                Console.WriteLine("すでに存在しています。");
            }

            //ファイルを削除する
            //Fileクラスを使った場合
            File.Delete(@"C:¥Example¥Greeting.txt");
            //FileInfoクラスを使った場合
            fi = new FileInfo(@"C:¥Example¥Greeting.txt");
            fi.Delete();

            // ファイルをコピーする
            //Fileクラスを使った場合
            //すでに存在する場合はIOException例外が発生
            File.Copy(@"C:¥Example¥Greeting.txt", @"C:¥Example¥Target.txt");
            //overwriteにtrueを設定すると上書き可能に
            File.Copy(@"C:¥Example¥Greeting.txt", @"C:¥Example¥Target.txt", overwrite:true);
            //FileInfoクラスを使った場合
            fi = new FileInfo(@"C:¥Example¥Greeting.txt");
            FileInfo dup = fi.CopyTo(@"C:¥Example¥Target.txt", overwrite: true);

            // ファイルを移動する
            //Fileクラスを使った場合
            // 異なるドライブ間の移動はサポートしていない
            File.Move(@"C:¥Example¥Greeting.txt", @"C:¥Sample¥Target.txt");
            //FileInfoクラスを使った場合
            // 異なるドライブ間の移動もサポートしている
            fi = new FileInfo(@"C:¥Example¥Greeting.txt");
            fi.MoveTo(@"C:¥Sample¥Target.txt");

            //ファイル名を変更する
            //移動先のパスに移動元と同じディレクトリを指定する
            //Fileクラスを使った場合
            File.Move(@"C:¥Example¥Greeting.txt", @"C:¥Example¥Target.txt");
            //FileInfoクラスを使った場合
            fi = new FileInfo(@"C:¥Example¥Greeting.txt");
            fi.MoveTo(@"C:¥Example¥Target.txt");

            //ファイルの最終更新日時/作成日時の取得
            //Fileクラスを使った場合
            var lastWriteTime = File.GetLastWriteTime(@"C:¥Example¥Greeting.txt");
            //FileInfoクラスを使った場合
            fi = new FileInfo(@"C:¥Example¥Greeting.txt");
            DateTime lastWriteTime = fi.LastWriteTime;

            //ファイルの最終更新日時/作成日時の設定
            //Fileクラスを使った場合
            File.SetLastWriteTime(@"C:¥Example¥Greeting.txt", DateTime.Now);
            //FileInfoクラスを使った場合
            fi = new FileInfo(@"C:¥Example¥Greeting.txt");
            fi.LastWriteTime = DateTime.Now;

            // ファイルのサイズを得る
            //Fileクラスを使った場合
            // できない
            //FileInfoクラスを使った場合
            fi = new FileInfo(@"C:¥Example¥Greeting.txt");
            long size = fi.Length;

            //通常はインスタンス生成の必要がないFileクラスを使い、
            //事前にFileInfoオブジェクトが求まっている場合は、FileInfoクラスのメソッドを利用する

            // 9.4 ディレクトリの操作
            //通常はインスタンス生成の必要がないDirectoryクラスを使い、
            //事前にDirectoryInfoオブジェクトが求まっている場合は、DirectoryInfoクラスのメソッドを利用する

            // ディレクトリの有無を調べる
            if (Directory.Exists(@"C:¥Example"))
            {
                Console.WriteLine("存在しています");
            } else{
                Console.WriteLine("存在していません")
            }

            // ディレクトリを作成する
            // Directoryクラスを使った場合
            // アクセス権がない場合やうこうなパス名を指定した場合は例外が発生
            // 戻り値は、作成されたディレクトリ情報を示すDirectoruInfoオブジェクト
            DirectoryInfo di = Directory.CreateDirectory(@"C:¥Example");

            // DirectoryInfoクラスを使った場合
            di = new DirectoryInfo(@"C:¥Example");
            di.Create();

            // ディレクトリを削除する
            // Directoryクラスを使った場合
            // ディレクトリの中が空の場合のみ削除される
            Directory.Delete(@"C:¥Example");
            // ディレクトリの中が空じゃなくても削除される
            Directory.Delete(@"C:¥Example¥temp", recursive:true);
            // DirectoryInfoクラスを使った場合
            di = new DirectoryInfo(@"C:¥Example");
            di.Delete(recursive: true);

            // ディレクトリを移動する
            // ディレクトリ名を変更する
            //指定フォルダにあるディレクトリの一覧を一度に取得する
            //指定フォルダにあるディレクトリの一覧を列挙する
            //指定フォルダにあるファイルの一覧を一度に取得する
            //指定フォルダにあるファイルの一覧を列挙する
            //ディレクトリとファイルの一覧を一緒に取得する
            //ディレクトリとファイルの更新日時を変更する

            // 9.5 パス名の操作
            // パス名を構成要素に分割する
            var path = @"C:¥Program Files¥Microsoft Office¥Office16¥EXCEL.EXE";
            var directoryName = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);
            var extension = Path.GetExtension(path);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var pathRoot = Path.GetPathRoot(path);

            // 相対パスから絶対パスを得る
            var fullPath = Path.GetFullPath(@"..¥Greeting.txt");

            // パスを組み立てる

            // 9.6 その他のファイル操作
            // 一時ファイルを作成する
            var tempFileName = Path.GetTempFileName();
            // 一時フォルダのパスを取得する
            var tempPath = Path.GetTempPath();
            // 特殊フォルダのパスを得る
            // デスクトップ
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        }
    }
}
