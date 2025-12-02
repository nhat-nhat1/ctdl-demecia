using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Decima
{
    internal class Program
    {
        struct quanly
        {
            public string Tensach;
            public string Tacgia;
            public int Namxuatban;
            public int Soluongcon;

            public quanly(string tensach, string tacgia, int namxuatban, int soluongcon)
            {
                Tensach = tensach;
                Tacgia = tacgia;
                Namxuatban = namxuatban;
                Soluongcon = soluongcon;
            }
        }
        static Dictionary<int, quanly> ss = new Dictionary<int, quanly>();
        static Dictionary<string, string> taikhoan = new Dictionary<string, string>();
        static Dictionary<int, quanly> dict = new Dictionary<int, quanly>();
        static int n, m;
        static int[] combo = new int[100];
        // Lưu vào thư mục chứa file exe
        static string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        static string accountFile = Path.Combine(dataFolder, "accounts.txt");

        // Hàm tạo khung đồng đều
        static void VeKhung(string title, int width = 50)
        {
            int padding = (width - 2 - title.Length) / 2;
            string top = "╔" + new string('═', width - 2) + "╗";
            string middle = "║" + new string(' ', padding) + title + new string(' ', width - 2 - title.Length - padding) + "║";
            string bottom = "╚" + new string('═', width - 2) + "╝";
            Console.WriteLine(top);
            Console.WriteLine(middle);
            Console.WriteLine(bottom);
        }

        // Hàm tạo khung menu đồng đều
        static void VeKhungMenu(int width = 50)
        {
            Console.WriteLine("┌" + new string('─', width - 2) + "┐");
        }

        static void VeDongMenu(string content, int width = 50)
        {
            Console.WriteLine("│  " + content.PadRight(width - 4) + "│");
        }

        static void VeKhungMenuCuoi(int width = 50)
        {
            Console.WriteLine("└" + new string('─', width - 2) + "┘");
        }

        // Hàm mã hóa mật khẩu (Base64)
        static string MaHoaMatKhau(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        // Hàm giải mã mật khẩu
        static string GiaiMaMatKhau(string encodedPassword)
        {
            if (string.IsNullOrEmpty(encodedPassword))
                return string.Empty;
            try
            {
                byte[] bytes = Convert.FromBase64String(encodedPassword);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return encodedPassword; // Nếu không giải mã được, trả về nguyên bản (tương thích với dữ liệu cũ)
            }
        }

        static void SetUTF8Console()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
        }

        static void LuuTaiKhoan()
        {
            try
            {
                if (!Directory.Exists(dataFolder))
                {
                    Directory.CreateDirectory(dataFolder);
                }

                using (StreamWriter writer = new StreamWriter(accountFile, false, Encoding.UTF8))
                {
                    foreach (var account in taikhoan)
                    {
                        // Lưu mật khẩu đã mã hóa
                        string encodedPassword = MaHoaMatKhau(account.Value);
                        writer.WriteLine($"{account.Key}|{encodedPassword}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"⚠️  Lỗi khi lưu tài khoản: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void TaiTaiKhoan()
        {
            try
            {
                if (File.Exists(accountFile))
                {
                    string[] lines = File.ReadAllLines(accountFile, Encoding.UTF8);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length == 2)
                            {
                                // Giải mã mật khẩu khi tải
                                string decodedPassword = GiaiMaMatKhau(parts[1]);
                                taikhoan[parts[0]] = decodedPassword;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️  Lỗi khi tải tài khoản: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void themsach()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("📚 THÊM SÁCH MỚI 📚", 50);
            Console.ResetColor();
            Console.Write("📖 Nhập số lượng sách bạn muốn thêm: ");
            int z = int.Parse(Console.ReadLine());
            for (int i = 0; i < z; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine($"📝 Sách thứ {i + 1}/{z}");
                Console.ResetColor();
                Console.Write("🔢 Nhập mã sách: ");
                int masach = int.Parse(Console.ReadLine());
                if (!dict.ContainsKey(masach))
                {
                    Console.Write("📖 Nhập tên sách: ");
                    string tensach = Console.ReadLine();
                    Console.Write("✍️  Nhập tác giả: ");
                    string tacgia = Console.ReadLine();
                    Console.Write("📅 Nhập năm xuất bản: ");
                    int namxuatban = int.Parse(Console.ReadLine());
                    Console.Write("📊 Nhập số lượng còn lại: ");
                    int soluongcon = int.Parse(Console.ReadLine());
                    if (soluongcon > 0)
                    {
                        dict.Add(masach, new quanly(tensach, tacgia, namxuatban, soluongcon));
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("✅ Đã thêm sách thành công!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Số lượng không hợp lệ, vui lòng nhập lại!");
                        Console.ResetColor();
                        i--;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Mã sách bị trùng, vui lòng nhập lại!");
                    Console.ResetColor();
                    i--;
                }
            }
            khonggica();
        }

        static void xemdanhsach()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("📚 DANH SÁCH SÁCH TRONG THƯ VIỆN 📚", 50);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"📊 Tổng số sách hiện có: {dict.Count} cuốn");
            Console.ResetColor();
            Console.WriteLine();

            var danhSach = dict.Values.ToList();
            int n = danhSach.Count;
            int i = 0;

            while (i < n)
            {
                var left = danhSach[i];
                string leftStr = "Mã Sách: " + (i + 1) + "\n" +
                                 "Tên Sách: " + left.Tensach + "\n" +
                                 "Tác Giả: " + left.Tacgia + "\n" +
                                 "Năm Xuất Bản: " + left.Namxuatban + "\n" +
                                 "Số lượng còn lại: " + left.Soluongcon + "\n";

                string rightStr = "";
                if (i + 1 < n)
                {
                    var right = danhSach[i + 1];
                    rightStr = "Mã Sách: " + (i + 2) + "\n" +
                               "Tên Sách: " + right.Tensach + "\n" +
                               "Tác Giả: " + right.Tacgia + "\n" +
                               "Năm Xuất Bản: " + right.Namxuatban + "\n" +
                               "Số lượng còn lại: " + right.Soluongcon + "\n";
                }

                string[] leftLines = leftStr.Split('\n');
                string[] rightLines = rightStr.Split('\n');

                int maxLines = leftLines.Length > rightLines.Length ? leftLines.Length : rightLines.Length;

                for (int j = 0; j < maxLines; j++)
                {
                    string leftLine = j < leftLines.Length ? leftLines[j] : "";
                    string rightLine = j < rightLines.Length ? rightLines[j] : "";
                    Console.WriteLine(leftLine.PadRight(50) + rightLine);
                }

                Console.WriteLine(new string('-', 100));
                i += 2;
            }

            khonggica();
        }


        static void timkiem()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("🔍 TÌM KIẾM SÁCH 🔍", 50);
            Console.ResetColor();
            bool timthay = false;
            Console.Write("🔢 Nhập mã sách bạn muốn tìm: ");
            int k = int.Parse(Console.ReadLine());
            foreach (var item in dict)
            {
                if (item.Key == k)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n✅ Tìm thấy sách mang mã số {k}");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    Console.ResetColor();
                    Console.WriteLine($"📖 Tên Sách: {item.Value.Tensach}");
                    Console.WriteLine($"✍️  Tác Giả: {item.Value.Tacgia}");
                    Console.WriteLine($"📅 Năm Xuất Bản: {item.Value.Namxuatban}");
                    Console.WriteLine($"📊 Số lượng còn lại: {item.Value.Soluongcon}");
                    timthay = true;
                    break;
                }
            }
            if (!timthay)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Không tìm thấy sách mang mã số {k}");
                Console.ResetColor();
            }
            khonggica();
        }

        static void sapxep()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("📊 SẮP XẾP SÁCH THEO NĂM XUẤT BẢN 📊", 50);
            Console.ResetColor();
            Console.WriteLine();
            var sapxep = dict.OrderBy(x => x.Value.Namxuatban).ToList();
            int n = sapxep.Count;
            int i = 0;

            while (i < n)
            {
                var left = sapxep[i];
                string leftStr = "Mã Sách: " + left.Key + "\n" +
                                 "Tên Sách: " + left.Value.Tensach + "\n" +
                                 "Tác Giả: " + left.Value.Tacgia + "\n" +
                                 "Năm Xuất Bản: " + left.Value.Namxuatban + "\n" +
                                 "Số lượng còn lại: " + left.Value.Soluongcon + "\n";

                string rightStr = "";
                if (i + 1 < n)
                {
                    var right = sapxep[i + 1];
                    rightStr = "Mã Sách: " + right.Key + "\n" +
                               "Tên Sách: " + right.Value.Tensach + "\n" +
                               "Tác Giả: " + right.Value.Tacgia + "\n" +
                               "Năm Xuất Bản: " + right.Value.Namxuatban + "\n" +
                               "Số lượng còn lại: " + right.Value.Soluongcon + "\n";
                }

                string[] leftLines = leftStr.Split('\n');
                string[] rightLines = rightStr.Split('\n');

                int maxLines = leftLines.Length > rightLines.Length ? leftLines.Length : rightLines.Length;

                for (int j = 0; j < maxLines; j++)
                {
                    string leftLine = j < leftLines.Length ? leftLines[j] : "";
                    string rightLine = j < rightLines.Length ? rightLines[j] : "";
                    Console.WriteLine(leftLine.PadRight(50) + rightLine);
                }

                Console.WriteLine(new string('-', 100));
                i += 2;
            }

            khonggica();
        }

        static void xoa()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("🗑️  XÓA SÁCH 🗑️", 51);
            Console.ResetColor();
            Console.Write("🔢 Nhập mã sách bạn muốn xóa: ");
            int h = int.Parse(Console.ReadLine());
            if (dict.ContainsKey(h))
            {
                dict.Remove(h);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Đã xóa thành công!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Không tồn tại mã sách {h}");
                Console.ResetColor();
            }
            khonggica();
        }

        static void lietkecombo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("📦 LIỆT KÊ BỘ SƯU TẬP SÁCH 📦", 50);
            Console.ResetColor();
            n = ss.Count;
            if (n == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  Bạn chưa lấy quyển sách nào!");
                Console.ResetColor();
                khonggica();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"📚 Có tổng cộng {n} đầu sách trong giỏ hàng.");
            Console.ResetColor();
            Console.Write("🔢 Nhập số lượng sách muốn chọn trong 1 bộ sưu tập (m): ");
            m = int.Parse(Console.ReadLine());

            if (m > n || m <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Số lượng m không hợp lệ, vui lòng nhập lại!");
                Console.ResetColor();
                khonggica();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"📦 Các bộ sưu tập sách có thể tạo ra (combo {m} sách):");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.ResetColor();

            List<int> danhsachma = new List<int>();
            foreach (var item in ss)
            {
                danhsachma.Add(item.Key);
            }
            danhsachma.Sort();

            BoSuutap(1, 0, danhsachma);

            khonggica();
        }

        static void BoSuutap(int i, int start, List<int> danhsachma)
        {
            for (int j = start; j < n; j++)
            {
                combo[i] = danhsachma[j];
                if (i == m)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("📦 Bộ sưu tập: ");
                    Console.ResetColor();
                    for (int k = 1; k <= m; k++)
                    {
                        int masach = combo[k];
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[{0}] ", ss[masach].Tensach);
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }
                else
                {
                    BoSuutap(i + 1, j + 1, danhsachma);
                }
            }
        }

        static void suasach()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("✏️  CẬP NHẬT SÁCH ✏️", 50);
            Console.ResetColor();
            Console.Write("🔢 Nhập mã sách muốn sửa: ");
            int b = int.Parse(Console.ReadLine());
            if (dict.ContainsKey(b))
            {
                Console.Write("📖 Nhập tên sách mới: ");
                string tensach = Console.ReadLine();
                Console.Write("✍️  Nhập tác giả mới: ");
                string tacgia = Console.ReadLine();
                Console.Write("📅 Nhập năm xuất bản mới: ");
                int namxuatban = int.Parse(Console.ReadLine());
                Console.Write("📊 Nhập số lượng còn lại: ");
                int soluongcon = int.Parse(Console.ReadLine());
                dict[b] = new quanly(tensach, tacgia, namxuatban, soluongcon);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Đã sửa thành công!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Không tìm thấy sách mang mã số {b}");
                Console.ResetColor();
            }
            khonggica();
        }

        static void khonggica()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n⏸️  Nhấn phím bất kỳ để quay lại menu...");
            Console.ResetColor();
            Console.ReadLine();
        }

        static void laytsach()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("📥 LẤY SÁCH 📥", 50);
            Console.ResetColor();
            Console.Write("📚 Số lượng sách bạn muốn lấy: ");
            int v = int.Parse(Console.ReadLine());
            for (int i = 0; i < v; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine($"📝 Sách thứ {i + 1}/{v}");
                Console.ResetColor();
                Console.Write("🔢 Mã số sách bạn muốn lấy: ");
                int id = int.Parse(Console.ReadLine());
                if (dict.ContainsKey(id))
                {
                    if (!ss.ContainsKey(id))
                    {
                        ss.Add(id, dict[id]);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("✅ Đã lấy thành công sách!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️  Sách này đã có trong giỏ!");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Không có sách mang mã số {id}");
                    Console.ResetColor();
                }
            }
            khonggica();
        }
        static void xemgiosach()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                VeKhung("🛒 GIỎ SÁCH CỦA BẠN 🛒", 50);
                Console.ResetColor();
                if (ss.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️  Giỏ sách hiện đang trống!");
                    Console.ResetColor();
                    khonggica();
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"📚 Số sách bạn đã lấy: {ss.Count} cuốn\n");
                Console.ResetColor();

                // Hiển thị danh sách sách
                int stt = 1;
                List<int> danhSachMa = new List<int>();
                foreach (var item in ss)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    Console.WriteLine($"📖 Sách #{stt}");
                    Console.ResetColor();
                    Console.WriteLine($"🔢 Mã Sách: {item.Key}");
                    Console.WriteLine($"📖 Tên Sách: {item.Value.Tensach}");
                    Console.WriteLine($"✍️  Tác Giả: {item.Value.Tacgia}");
                    Console.WriteLine($"📅 Năm Xuất Bản: {item.Value.Namxuatban}");
                    Console.WriteLine($"📊 Số lượng còn lại: {item.Value.Soluongcon}");
                    Console.WriteLine();
                    danhSachMa.Add(item.Key);
                    stt++;
                }

                // Menu chức năng
                Console.ForegroundColor = ConsoleColor.Cyan;
                VeKhungMenu(50);
                VeDongMenu("1. 🗑️  Xóa sách khỏi giỏ", 51);
                VeDongMenu("2. 🗑️  Xóa tất cả sách", 51);
                VeDongMenu("3. ↩️  Quay lại menu chính", 50);
                VeKhungMenuCuoi(50);
                Console.ResetColor();
                Console.Write("👉 Chọn chức năng: ");

                try
                {
                    int chon = int.Parse(Console.ReadLine());
                    switch (chon)
                    {
                        case 1:
                            XoaSachKhoiGio(danhSachMa);
                            break;
                        case 2:
                            XoaTatCaSach();
                            break;
                        case 3:
                            return;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                            Console.ResetColor();
                            Console.ReadKey();
                            break;
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Vui lòng nhập số hợp lệ!");
                    Console.ResetColor();
                    Console.ReadKey();
                }
            }
        }

        static void XoaSachKhoiGio(List<int> danhSachMa)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("🗑️  XÓA SÁCH KHỎI GIỎ 🗑️", 50);
            Console.ResetColor();
            Console.Write("🔢 Nhập mã sách muốn xóa: ");
            try
            {
                int masach = int.Parse(Console.ReadLine());
                if (ss.ContainsKey(masach))
                {
                    string tenSach = ss[masach].Tensach;
                    ss.Remove(masach);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✅ Đã xóa sách \"{tenSach}\" khỏi giỏ!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Không tìm thấy sách có mã {masach} trong giỏ!");
                    Console.ResetColor();
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Mã sách không hợp lệ!");
                Console.ResetColor();
            }
            Console.WriteLine("\n⏸️  Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        static void XoaTatCaSach()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("🗑️  XÓA TẤT CẢ SÁCH 🗑️", 50);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Bạn có chắc muốn xóa tất cả {ss.Count} cuốn sách khỏi giỏ?");
            Console.ResetColor();
            Console.Write("👉 Nhập 'YES' để xác nhận: ");
            string xacNhan = Console.ReadLine();
            if (xacNhan.ToUpper() == "YES")
            {
                int soLuong = ss.Count;
                ss.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Đã xóa {soLuong} cuốn sách khỏi giỏ!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("❌ Đã hủy thao tác!");
                Console.ResetColor();
            }
            Console.WriteLine("\n⏸️  Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        static void DangKy()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("📝 ĐĂNG KÝ TÀI KHOẢN 📝", 50);
            Console.ResetColor();

            Console.Write("👤 Username: ");
            string user = Console.ReadLine();
            if (taikhoan.ContainsKey(user))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Username đã tồn tại!");
                Console.ResetColor();
                khonggica();
                return;
            }

            Console.Write("🔒 Password: ");
            string pass = Console.ReadLine();
            taikhoan.Add(user, pass);
            LuuTaiKhoan(); // Lưu tài khoản vào file

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Đăng ký thành công!");
            Console.ResetColor();
            khonggica();
        }

        static bool DangNhap()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VeKhung("🔐 ĐĂNG NHẬP HỆ THỐNG 🔐", 50);
            Console.ResetColor();

            Console.Write("👤 Username: ");
            string user = Console.ReadLine();
            Console.Write("🔒 Password: ");
            string pass = Console.ReadLine();

            if (taikhoan.ContainsKey(user) && taikhoan[user] == pass)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Đăng nhập thành công!");
                Console.ResetColor();
                khonggica();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Sai tài khoản hoặc mật khẩu!");
                Console.ResetColor();
                khonggica();
                return false;
            }
        }
        static void Tao10000SachThat()
        {
            Console.Clear();
            VeKhung("ĐANG KHỞI TẠO 10.000 CUỐN SÁCH THẬT", 70);
            Console.WriteLine("Vui lòng chờ một chút...\n");

            var watch = Stopwatch.StartNew();
            Random r = new Random();

            string[] tacgia = { "Nguyễn Văn", "Trần Thị", "Lê Văn", "Phạm Thị", "Hoàng Văn", "Đặng Thị", "Bùi Văn", "Đỗ Thị", "Ngô Văn", "Vũ Thị" };
            string[] chude = { "Lập trình", "Cơ sở dữ liệu", "Mạng máy tính", "AI", "Khoa học dữ liệu", "An ninh mạng", "Web", "Mobile", "Thuật toán", "Machine Learning", "Blockchain", "Cloud", "IoT", "Game", "Hệ điều hành" };

            int maBatDau = dict.Keys.DefaultIfEmpty(0).Max() + 1;

            for (int i = 0; i < 10000; i++)
            {
                int masach = maBatDau + i;
                string ten = $"{chude[r.Next(chude.Length)]} - Tập {r.Next(1, 31)}";
                string tg = $"{tacgia[r.Next(tacgia.Length)]} {(char)('A' + r.Next(26))}";
                int nam = 1990 + r.Next(36);
                int sl = r.Next(1, 101);

                dict[masach] = new quanly(ten, tg, nam, sl);
            }

            watch.Stop();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("HOÀN TẤT!");
            Console.WriteLine($"Đã thêm 10.000 cuốn sách vào thư viện!");
            Console.WriteLine($"Tổng sách hiện tại: {dict.Count:N0} cuốn");
            Console.WriteLine($"Thời gian thực hiện: {watch.ElapsedMilliseconds} ms");
            Console.ResetColor();
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        static void TrangDangNhap()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                VeKhung("📚 HỆ THỐNG QUẢN LÝ THƯ VIỆN SÁCH 📚", 50);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                VeKhungMenu(50);
                VeDongMenu("1. 🔐 Đăng nhập", 50);
                VeDongMenu("2. 📝 Đăng ký", 50);
                VeDongMenu("3. 🚪 Thoát", 50);
                VeKhungMenuCuoi(50);
                Console.ResetColor();
                Console.Write("👉 Chọn chức năng: ");
                int chon = int.Parse(Console.ReadLine());
                switch (chon)
                {
                    case 1:
                        if (DangNhap()) return;
                        break;
                    case 2:
                        DangKy();
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("👋 Cảm ơn bạn đã sử dụng hệ thống!");
                        Console.ResetColor();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                        Console.ResetColor();
                        khonggica();
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            SetUTF8Console();
            TaiTaiKhoan(); // Tải tài khoản từ file khi khởi động
            if (!taikhoan.ContainsKey("admin"))
            {
                taikhoan.Add("admin", "123");
                LuuTaiKhoan(); // Lưu tài khoản admin mặc định
            }
            TrangDangNhap();
            dict.Add(1, new quanly("Lập trình C#", "Nguyễn Văn A", 2020, 50));
            dict.Add(2, new quanly("Cấu trúc dữ liệu", "Trần Thị B", 2018, 40));
            dict.Add(3, new quanly("Thuật toán nâng cao", "Lê Văn C", 2019, 30));
            dict.Add(4, new quanly("Hệ điều hành", "Phạm Thị D", 2017, 25));
            dict.Add(5, new quanly("Cơ sở dữ liệu", "Ngô Văn E", 2021, 60));
            dict.Add(6, new quanly("Mạng máy tính", "Đặng Thị F", 2016, 35));
            dict.Add(7, new quanly("Java cơ bản", "Nguyễn Văn G", 2020, 45));
            dict.Add(8, new quanly("Python cho người mới", "Trần Thị H", 2021, 55));
            dict.Add(9, new quanly("Thiết kế web", "Lê Văn I", 2019, 40));
            dict.Add(10, new quanly("Lập trình Android", "Phạm Thị J", 2020, 50));
            dict.Add(11, new quanly("Lập trình iOS", "Ngô Văn K", 2021, 30));
            dict.Add(12, new quanly("AI cơ bản", "Đặng Thị L", 2022, 20));
            dict.Add(13, new quanly("Machine Learning", "Nguyễn Văn M", 2022, 25));
            dict.Add(14, new quanly("Deep Learning", "Trần Thị N", 2022, 15));
            dict.Add(15, new quanly("Phân tích dữ liệu", "Lê Văn O", 2021, 40));
            dict.Add(16, new quanly("Big Data", "Phạm Thị P", 2020, 35));
            dict.Add(17, new quanly("Blockchain cơ bản", "Ngô Văn Q", 2022, 20));
            dict.Add(18, new quanly("An ninh mạng", "Đặng Thị R", 2019, 25));
            dict.Add(19, new quanly("Thiết kế UX/UI", "Nguyễn Văn S", 2021, 30));
            dict.Add(20, new quanly("Kiến trúc phần mềm", "Trần Thị T", 2020, 40));
            dict.Add(21, new quanly("C++ toàn tập", "Lê Văn U", 2017, 45));
            dict.Add(22, new quanly("Phân tích hệ thống", "Phạm Thị V", 2018, 50));
            dict.Add(23, new quanly("Visual Basic cơ bản", "Ngô Văn W", 2016, 25));
            dict.Add(24, new quanly("Toán rời rạc", "Đặng Thị X", 2019, 30));
            dict.Add(25, new quanly("Lý thuyết đồ thị", "Nguyễn Văn Y", 2021, 35));
            dict.Add(26, new quanly("Phân tích thuật toán", "Trần Thị Z", 2020, 40));
            dict.Add(27, new quanly("Khoa học máy tính nhập môn", "Lê Văn A1", 2015, 20));
            dict.Add(28, new quanly("Phát triển phần mềm Agile", "Phạm Thị B1", 2021, 60));
            dict.Add(29, new quanly("Kiểm thử phần mềm", "Ngô Văn C1", 2020, 30));
            dict.Add(30, new quanly("Quản lý dự án CNTT", "Đặng Thị D1", 2018, 40));
            dict.Add(31, new quanly("Cloud Computing", "Nguyễn Văn E1", 2022, 50));
            dict.Add(32, new quanly("IOT cơ bản", "Trần Thị F1", 2021, 45));
            dict.Add(33, new quanly("Điện toán đám mây nâng cao", "Lê Văn G1", 2020, 35));
            dict.Add(34, new quanly("Phát triển game Unity", "Phạm Thị H1", 2022, 25));
            dict.Add(35, new quanly("Thiết kế hệ thống lớn", "Ngô Văn I1", 2023, 20));
            dict.Add(36, new quanly("Học máy ứng dụng", "Đặng Thị J1", 2023, 30));
            dict.Add(37, new quanly("Cấu trúc máy tính", "Nguyễn Văn K1", 2019, 40));
            dict.Add(38, new quanly("Xử lý ảnh số", "Trần Thị L1", 2021, 35));
            dict.Add(39, new quanly("Thị giác máy tính", "Lê Văn M1", 2022, 20));
            dict.Add(40, new quanly("Tư duy lập trình", "Phạm Thị N1", 2020, 60));
            dict.Add(41, new quanly("Phát triển phần mềm nhúng", "Ngô Văn O1", 2019, 25));
            dict.Add(42, new quanly("Kỹ năng mềm cho IT", "Đặng Thị P1", 2020, 40));
            dict.Add(43, new quanly("Lập trình vi điều khiển", "Nguyễn Văn Q1", 2017, 50));
            dict.Add(44, new quanly("Quản trị mạng", "Trần Thị R1", 2021, 45));
            dict.Add(45, new quanly("An toàn thông tin", "Lê Văn S1", 2022, 25));
            dict.Add(46, new quanly("Thiết kế cơ sở dữ liệu", "Phạm Thị T1", 2020, 30));
            dict.Add(47, new quanly("Hệ quản trị SQL", "Ngô Văn U1", 2018, 35));
            dict.Add(48, new quanly("SQL nâng cao", "Đặng Thị V1", 2021, 50));
            dict.Add(49, new quanly("Phân tích thiết kế hướng đối tượng", "Nguyễn Văn W1", 2019, 40));
            dict.Add(50, new quanly("Lập trình hướng đối tượng", "Trần Thị X1", 2020, 55));
            dict.Add(51, new quanly("Thiết kế giao diện người dùng", "Lê Văn Y1", 2021, 25));
            dict.Add(52, new quanly("Java nâng cao", "Phạm Thị Z1", 2022, 45));
            dict.Add(53, new quanly("Python nâng cao", "Ngô Văn A2", 2023, 35));
            dict.Add(54, new quanly("C# nâng cao", "Đặng Thị B2", 2021, 40));
            dict.Add(55, new quanly("Phát triển web với ASP.NET", "Nguyễn Văn C2", 2022, 50));
            dict.Add(56, new quanly("Phát triển web với Django", "Trần Thị D2", 2023, 60));
            dict.Add(57, new quanly("Thiết kế phần mềm", "Lê Văn E2", 2019, 20));
            dict.Add(58, new quanly("Kiến trúc hệ thống phân tán", "Phạm Thị F2", 2021, 45));
            dict.Add(59, new quanly("Kỹ thuật lập trình", "Ngô Văn G2", 2018, 40));
            dict.Add(60, new quanly("Lập trình mạng", "Đặng Thị H2", 2020, 30));
            dict.Add(61, new quanly("Phát triển ứng dụng web", "Nguyễn Văn I2", 2022, 25));
            dict.Add(62, new quanly("Ứng dụng Python trong khoa học dữ liệu", "Trần Thị J2", 2021, 50));
            dict.Add(63, new quanly("AI nâng cao", "Lê Văn K2", 2023, 40));
            dict.Add(64, new quanly("Lập trình R cơ bản", "Phạm Thị L2", 2020, 35));
            dict.Add(65, new quanly("Phân tích thống kê", "Ngô Văn M2", 2019, 30));
            dict.Add(66, new quanly("Xử lý ngôn ngữ tự nhiên", "Đặng Thị N2", 2022, 20));
            dict.Add(67, new quanly("AI ứng dụng trong công nghiệp", "Nguyễn Văn O2", 2023, 25));
            dict.Add(68, new quanly("Lập trình web với React", "Trần Thị P2", 2022, 30));
            dict.Add(69, new quanly("Lập trình web với VueJS", "Lê Văn Q2", 2021, 40));
            dict.Add(70, new quanly("Lập trình Flutter", "Phạm Thị R2", 2023, 50));
            dict.Add(71, new quanly("Phát triển mobile đa nền tảng", "Ngô Văn S2", 2021, 35));
            dict.Add(72, new quanly("Tối ưu thuật toán", "Đặng Thị T2", 2022, 20));
            dict.Add(73, new quanly("Khoa học dữ liệu nâng cao", "Nguyễn Văn U2", 2023, 30));
            dict.Add(74, new quanly("Điện toán lượng tử", "Trần Thị V2", 2024, 25));
            dict.Add(75, new quanly("Nguyên lý máy học", "Lê Văn W2", 2022, 35));
            dict.Add(76, new quanly("Phân tích chuỗi thời gian", "Phạm Thị X2", 2020, 30));
            dict.Add(77, new quanly("Kỹ thuật phần mềm hiện đại", "Ngô Văn Y2", 2021, 40));
            dict.Add(78, new quanly("Robot học cơ bản", "Đặng Thị Z2", 2023, 50));
            dict.Add(79, new quanly("Kỹ thuật lập trình nâng cao", "Nguyễn Văn A3", 2020, 45));
            dict.Add(80, new quanly("Phân tích hình ảnh y tế", "Trần Thị B3", 2022, 25));
            dict.Add(81, new quanly("Học sâu cho người mới", "Lê Văn C3", 2023, 35));
            dict.Add(82, new quanly("AI trong nông nghiệp", "Phạm Thị D3", 2022, 30));
            dict.Add(83, new quanly("AI trong y học", "Ngô Văn E3", 2024, 25));
            dict.Add(84, new quanly("AI trong giáo dục", "Đặng Thị F3", 2024, 50));
            dict.Add(85, new quanly("Phân tích cảm xúc", "Nguyễn Văn G3", 2023, 35));
            dict.Add(86, new quanly("Phân loại hình ảnh", "Trần Thị H3", 2022, 20));
            dict.Add(87, new quanly("Phát hiện khuôn mặt", "Lê Văn I3", 2021, 25));
            dict.Add(88, new quanly("Nhận dạng giọng nói", "Phạm Thị J3", 2020, 30));
            dict.Add(89, new quanly("Chatbot thông minh", "Ngô Văn K3", 2024, 50));
            dict.Add(90, new quanly("AI và đạo đức", "Đặng Thị L3", 2023, 30));
            dict.Add(91, new quanly("AI trong marketing", "Nguyễn Văn M3", 2022, 40));
            dict.Add(92, new quanly("AI trong thương mại điện tử", "Trần Thị N3", 2021, 35));
            dict.Add(93, new quanly("AI trong tài chính", "Lê Văn O3", 2023, 25));
            dict.Add(94, new quanly("AI trong sản xuất", "Phạm Thị P3", 2024, 20));
            dict.Add(95, new quanly("AI và tương lai lao động", "Ngô Văn Q3", 2024, 30));
            dict.Add(96, new quanly("AI trong nghệ thuật", "Đặng Thị R3", 2023, 35));
            dict.Add(97, new quanly("Tư duy dữ liệu", "Nguyễn Văn S3", 2022, 45));
            dict.Add(98, new quanly("Kỹ năng phân tích dữ liệu", "Trần Thị T3", 2021, 50));
            dict.Add(99, new quanly("Xử lý dữ liệu lớn", "Lê Văn U3", 2020, 40));
            dict.Add(100, new quanly("Kho dữ liệu và BI", "Phạm Thị V3", 2021, 60));


            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                VeKhung("📚 MENU QUẢN LÝ SÁCH 📚", 50);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                VeKhungMenu(50);
                VeDongMenu("1. 📋 Xem danh sách sách", 50);
                VeDongMenu("2. ➕ Thêm sách", 49);
                VeDongMenu("3. 📊 Sắp xếp sách (theo năm xuất bản)", 50);
                VeDongMenu("4. 🗑️  Xóa sách", 51);
                VeDongMenu("5. 🔍 Tìm kiếm sách (theo mã)", 50);
                VeDongMenu("6. 📦 Liệt kê các bộ sưu tập sách", 50);
                VeDongMenu("7. ✏️  Cập nhật thông tin sách", 50);
                VeDongMenu("8. 📥 Lấy sách", 50);
                VeDongMenu("9. 🛒 Xem sách đã lấy", 50);
                VeDongMenu("10. 🔎 Kiểm tra khả năng khởi tạo 10.000 sách", 50);
                VeDongMenu("11. 🚪 Thoát", 50);
                VeKhungMenuCuoi(50);
                Console.ResetColor();
                Console.Write("👉 Chọn chức năng: ");
                int chon = int.Parse(Console.ReadLine());
                switch (chon)
                {
                    case 1: xemdanhsach(); break;
                    case 2: themsach(); break;
                    case 3: sapxep(); break;
                    case 4: xoa(); break;
                    case 5: timkiem(); break;
                    case 6: lietkecombo(); break;
                    case 7: suasach(); break;
                    case 8: laytsach(); break;
                    case 9: xemgiosach(); break;
                    case 10: Tao10000SachThat(); break;
                    case 11:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("👋 Cảm ơn bạn đã sử dụng hệ thống!");
                        Console.ResetColor();
                        LuuTaiKhoan(); // Lưu tài khoản trước khi thoát
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Lựa chọn không hợp lệ! Nhập từ 1-10.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
