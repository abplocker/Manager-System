using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
namespace HeThongTichHop
{
    public partial class frm_Main : Form
    {
        // Thay đổi link database ở đây
        // MySQL
        MySqlConnection mysqlconn = new MySqlConnection("server=localhost;user=root;database=mydb;port=3306;password=quangthien");
        //SQL Sever
        SqlConnection sqlconn = new SqlConnection("Data Source=DESKTOP-CN58EJ1;Initial Catalog=HR;Persist Security Info=True;User ID=sa;Password=12345");
        // Data
        double thunhap_codong_co = 0;
        double thunhap_codong_khong = 0;
        double thunhap_nam = 0;
        double thunhap_nu = 0;
        double thunhap_parttime = 0;
        double thunhap_fulltime = 0;
        double phucloi_codong_co = 0;
        double phucloi_codong_khong = 0;
        public frm_Main()
        {
            InitializeComponent();
        }
        private void btn_NV_TimKiem_Click(object sender, EventArgs e)
        {
            // ô text box trống thì ...
            if (string.IsNullOrWhiteSpace(txt_NV_Tennhanvien.Text))
            {
                MessageBox.Show("khong co thong tin");
            }
            // ô text box không trống thì ...
            else if (string.IsNullOrWhiteSpace(txt_NV_Tennhanvien.Text) == false)
            {
                MessageBox.Show("Khong co nguoi nay");

            }
        }
        private void load_gv_ThuNhap()
        {
            gv_ThuNhap.Rows.Clear();
            sqlconn.Open();
            // Đọc dữ liệu từ sql
            // Đếm số lượng nhân viên trong personal
            SqlCommand comm_Count_Personal = new SqlCommand("Select Count (*) from Personal", sqlconn);
            int count = (int)comm_Count_Personal.ExecuteScalar();
            sqlconn.Close();
            sqlconn.Open();
            // Đọc từ table Personal
            SqlCommand comm_Personal = new SqlCommand("Select * from Personal", sqlconn);
            SqlDataReader rd_Personal = comm_Personal.ExecuteReader();
            while (rd_Personal.Read())
            {
                int employee = int.Parse(rd_Personal["Employee_ID"].ToString());
                string ten = rd_Personal["First_Name"].ToString() + " " + rd_Personal["Last_Name"].ToString();
                string codong = (bool)rd_Personal["Shareholder_Status"] ? "Có" : "Không";
                string gioitinh = (bool)rd_Personal["Gender"] ? "Nam" : "Nữ";
                string dantoc = rd_Personal["Ethnicity"].ToString();
                gv_ThuNhap.Rows.Add(employee, "", ten, "", "", "", codong, gioitinh, dantoc, "");
            }
            // Đóng reader của Personal
            rd_Personal.Close();
            for (int i = 0; i < count; i++)
            {
                // Đọc từ table Employment
                int id = int.Parse(gv_ThuNhap[0, i].Value.ToString());
                SqlCommand comm_Employment = new SqlCommand("Select * from Employment where Employee_ID ='" + id.ToString() + "'", sqlconn);
                SqlDataReader rd_Employment = comm_Employment.ExecuteReader();
                rd_Employment.Read();
                string loainhanvien = rd_Employment["Employment_Status"].ToString();
                gv_ThuNhap[9, i].Value = loainhanvien;
                // Đóng reader của Employment
                rd_Employment.Close();
            }
            //Đóng kết nối với sql
            sqlconn.Close();
            // Mở kết nối với mysql
            mysqlconn.Open();
            for (int i = 0; i < count; i++)
            {
                int id = int.Parse(gv_ThuNhap[0, i].Value.ToString());
                // Đọc dữ liệu từ mysql
                MySqlCommand comm_employee = new MySqlCommand("SELECT * FROM employee where idEmployee ='" + id.ToString() + "'", mysqlconn);
                MySqlDataReader rd_employee = comm_employee.ExecuteReader();
                rd_employee.Read();
                // Đọc dữ liệu từ bảng employee
                string denthoidiemhientai = rd_employee["Paid To Date"].ToString();
                string namtruoc = rd_employee["Paid Last Year"].ToString();
                int idPay = int.Parse(rd_employee["Pay Rates_idPay Rates"].ToString());
                gv_ThuNhap[1, i].Value = idPay;
                gv_ThuNhap[4, i].Value = denthoidiemhientai;
                gv_ThuNhap[5, i].Value = namtruoc;
                rd_employee.Close();
            }
            for (int i = 0; i < count; i++)
            {
                int idPay = int.Parse(gv_ThuNhap[1, i].Value.ToString());
                //Đọc dữ liệu từ mysql
                // chú ý ở mysql nếu như tên trường có dấu cách thì dùng dấu `` để nhóm lại, không dùng dấu ''
                MySqlCommand comm_payrates = new MySqlCommand("SELECT * FROM `pay rates` where `idPay Rates` = '" + idPay.ToString() + "'", mysqlconn);
                MySqlDataReader rd_payrates = comm_payrates.ExecuteReader();
                rd_payrates.Read();
                // Đọc dữ liệu từ bảng pay rates
                string thunhap = rd_payrates["Pay Amount"].ToString();
                gv_ThuNhap[3, i].Value = thunhap;
                rd_payrates.Close();
            }
            for (int i = 0; i < count; i++)
            {
                if (gv_ThuNhap[6,i].Value.ToString() == "Có")
                    thunhap_codong_co += double.Parse(gv_ThuNhap[3,i].Value.ToString());
                else
                    thunhap_codong_khong += double.Parse(gv_ThuNhap[3, i].Value.ToString());
                if (gv_ThuNhap[7, i].Value.ToString() == "Nam")
                    thunhap_nam += double.Parse(gv_ThuNhap[3, i].Value.ToString());
                else
                    thunhap_nu += double.Parse(gv_ThuNhap[3, i].Value.ToString());
                if (gv_ThuNhap[9, i].Value.ToString().ToLower() == "toàn thời gian")
                    thunhap_fulltime += double.Parse(gv_ThuNhap[3, i].Value.ToString());
                else
                    thunhap_parttime += double.Parse(gv_ThuNhap[3, i].Value.ToString());
            }
            mysqlconn.Close();
        }
        private void load_gv_PhucLoi()
        {
            gv_PhucLoi.Rows.Clear();
            sqlconn.Open();
            // Đọc dữ liệu từ sql
            // Đếm số lượng nhân viên trong personal
            SqlCommand comm_Count_Personal = new SqlCommand("Select Count (*) from Personal", sqlconn);
            int count = (int)comm_Count_Personal.ExecuteScalar();
            sqlconn.Close();
            sqlconn.Open();
            // Đọc từ table Personal
            SqlCommand comm_Personal = new SqlCommand("Select * from Personal", sqlconn);
            SqlDataReader rd_Personal = comm_Personal.ExecuteReader();
            while (rd_Personal.Read())
            {
                int employee = int.Parse(rd_Personal["Employee_ID"].ToString());
                int benefit_plans = int.Parse(rd_Personal["Benefit_Plans"].ToString());
                string ten = rd_Personal["First_Name"].ToString() + " " + rd_Personal["Last_Name"].ToString();
                string codong = (bool)rd_Personal["Shareholder_Status"] ? "Có" : "Không";
                gv_PhucLoi.Rows.Add(employee, benefit_plans, ten,"", codong, "");
            }
            // Đóng reader của Personal
            rd_Personal.Close();
            for (int i = 0; i < count; i++)
            {
                // Đọc từ table Employment
                int benefit_plans = int.Parse(gv_PhucLoi[1, i].Value.ToString());
                SqlCommand comm_Employment = new SqlCommand("Select * from Benefit_Plans where Benefit_Plan_ID ='" + benefit_plans.ToString() + "'", sqlconn);
                SqlDataReader rd_Employment = comm_Employment.ExecuteReader();
                rd_Employment.Read();
                string phucloitra = rd_Employment["Deductable"].ToString();
                string tenphucloi = rd_Employment["Plan_Name"].ToString();
                gv_PhucLoi[3, i].Value = tenphucloi;
                gv_PhucLoi[5, i].Value = phucloitra;
                // Đóng reader của Employment
                rd_Employment.Close();
            }
            //Đóng kết nối với sql
            sqlconn.Close();
            for (int i = 0; i < count; i++)
            {
                if (gv_PhucLoi[4, i].Value.ToString() == "Có")
                    phucloi_codong_co += double.Parse(gv_PhucLoi[5, i].Value.ToString());
                else
                    phucloi_codong_khong += double.Parse(gv_PhucLoi[5, i].Value.ToString());
            }
            chart_phucloi.Series["Benefits"].Points.Clear();
            chart_phucloi.Series["Benefits"].Points.AddXY("Cổ đông", phucloi_codong_co);
            chart_phucloi.Series["Benefits"].Points.AddXY("Không cổ đông", phucloi_codong_khong);
        }

        private void load_gv_SoNgayNghi()
        {
            gv_SoNgayNghi.Rows.Clear();
            sqlconn.Open();
            // Đọc dữ liệu từ sql
            // Đếm số lượng nhân viên trong personal
            SqlCommand comm_Count_Personal = new SqlCommand("Select Count (*) from Personal", sqlconn);
            int count = (int)comm_Count_Personal.ExecuteScalar();
            sqlconn.Close();
            sqlconn.Open();
            // Đọc từ table Personal
            SqlCommand comm_Personal = new SqlCommand("Select * from Personal", sqlconn);
            SqlDataReader rd_Personal = comm_Personal.ExecuteReader();
            while (rd_Personal.Read())
            {
                int employee = int.Parse(rd_Personal["Employee_ID"].ToString());
                int vacationDays = 0;
                string ten = rd_Personal["First_Name"].ToString() + " " + rd_Personal["Last_Name"].ToString();
                string codong = (bool)rd_Personal["Shareholder_Status"] ? "Có" : "Không";
                string gioitinh = (bool)rd_Personal["Gender"] ? "Nam" : "Nữ";
                string dantoc = rd_Personal["Ethnicity"].ToString();
                gv_SoNgayNghi.Rows.Add(employee, vacationDays, ten, "[3]Số ngày nghỉ", "[4]Số ngày được nghỉ","5","6", codong, gioitinh, dantoc, "10");
            }
            // Đóng reader của Personal
            rd_Personal.Close();
            for (int i = 0; i < count; i++)
            {
                // Đọc từ table Employment
                int id = int.Parse(gv_ThuNhap[0, i].Value.ToString());
                SqlCommand comm_Employment = new SqlCommand("Select * from Employment where Employee_ID ='" + id.ToString() + "'", sqlconn);
                SqlDataReader rd_Employment = comm_Employment.ExecuteReader();
                rd_Employment.Read();
                string loainhanvien = rd_Employment["Employment_Status"].ToString();
                gv_SoNgayNghi[9, i].Value = loainhanvien;
                // Đóng reader của Employment
                rd_Employment.Close();
            }
            //Đóng kết nối với sql
            sqlconn.Close();
            // Mở kết nối với mysql
            mysqlconn.Open();
            for (int i = 0; i < count; i++)
            {
                int id = int.Parse(gv_ThuNhap[0, i].Value.ToString());
                // Đọc dữ liệu từ mysql
                MySqlCommand comm_employee = new MySqlCommand("SELECT * FROM employee where idEmployee ='" + id.ToString() + "'", mysqlconn);
                MySqlDataReader rd_employee = comm_employee.ExecuteReader();
                rd_employee.Read();
                // Đọc dữ liệu từ bảng employee
                string denthoidiemhientai = rd_employee["Paid To Date"].ToString();
                string namtruoc = rd_employee["Paid Last Year"].ToString();
                string songayduocnghi = rd_employee["Vacation Days"].ToString();
                gv_SoNgayNghi[4, i].Value = songayduocnghi;
                gv_SoNgayNghi[5, i].Value = denthoidiemhientai;
                gv_SoNgayNghi[6, i].Value = namtruoc;
                rd_employee.Close();
            }
            mysqlconn.Close();
        } // load gv_SoNgayNghi

        /*
         * Bảng thông báo
         */
        private void load_gv_KiNiem()
        {
            gv_KiNiem.Rows.Clear();
            sqlconn.Open();
            // Đọc dữ liệu từ sql
            // Đếm số lượng nhân viên trong personal
            SqlCommand comm_Count_Personal = new SqlCommand("Select Count (*) from Personal", sqlconn);
            int count = (int)comm_Count_Personal.ExecuteScalar();
            sqlconn.Close();
            sqlconn.Open();
            // Đọc từ table Personal
            SqlCommand comm_Personal = new SqlCommand("Select * from Personal", sqlconn);
            SqlDataReader rd_Personal = comm_Personal.ExecuteReader();
            while (rd_Personal.Read())
            {
                int employee = int.Parse(rd_Personal["Employee_ID"].ToString());
                string ten = rd_Personal["First_Name"].ToString() + " " + rd_Personal["Last_Name"].ToString();
                gv_KiNiem.Rows.Add(employee,ten,"Ngày thuê","Năm kỉ niệm");
            }
            // Đóng reader của Personal
            rd_Personal.Close();
            for (int i = 0; i < count; i++)
            {
                // Đọc từ table Employment
                int id = int.Parse(gv_KiNiem[0, i].Value.ToString());
                SqlCommand comm_Employment = new SqlCommand("Select * from Employment where Employee_ID ='" + id.ToString() + "'", sqlconn);
                SqlDataReader rd_Employment = comm_Employment.ExecuteReader();
                rd_Employment.Read();
                string ngaythue_text = rd_Employment["Hire_Date"].ToString();
                DateTime ngaythue = DateTime.Parse(ngaythue_text);
                gv_KiNiem[2, i].Value = ngaythue.ToString("dd/MM/yyyy");
                DateTime now = DateTime.Now;
                int namkiniem = now.Year - ngaythue.Year;
                gv_KiNiem[3, i].Value = namkiniem.ToString() + " năm";
                // Đóng reader của Employment
                rd_Employment.Close();
            }
            //Đóng kết nối với sql
            sqlconn.Close();
        }
        private void tabControl_ThongBao_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    load_gv_KiNiem();
                    break;
            }
        }
        private void frm_Main_Load(object sender, EventArgs e)
        {
            tabcontrol_Baocao.SelectedIndexChanged += new EventHandler(tabcontrol_BaoCao_SelectedIndexChanged);
            tabControl_ThongBao.SelectedIndexChanged += new EventHandler(tabControl_ThongBao_SelectedIndexChanged);
            load_gv_ThuNhap();
            load_gv_KiNiem();
            radio_thunhap_codong_CheckedChanged(sender, e);
        }

        private void cb_NamKiNiem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_NamKiNiem.SelectedItem.ToString() == "")
            {
                for (int i = 0; i < gv_KiNiem.RowCount - 1; i++)
                    gv_KiNiem.Rows[i].Visible = true;
            }
            else
            {
                for (int i = 0; i < gv_KiNiem.RowCount - 1; i++)
                    gv_KiNiem.Rows[i].Visible = true;
                for (int i = 0; i < gv_KiNiem.RowCount - 1; i++)
                {
                    if (gv_KiNiem[3, i].Value.ToString() != cb_NamKiNiem.SelectedItem.ToString())
                        gv_KiNiem.Rows[i].Visible = false;
                }
            }
        }

        private void radio_thunhap_codong_CheckedChanged(object sender, EventArgs e)
        {
            chart_ThuNhap.Series["Earnings"].Points.Clear();
            chart_ThuNhap.Series["Earnings"].Points.AddXY("Cổ đông", thunhap_codong_co);
            chart_ThuNhap.Series["Earnings"].Points.AddXY("Không cổ đông", thunhap_codong_khong);
        }

        private void radio_thunhap_gioitinh_CheckedChanged(object sender, EventArgs e)
        {
            chart_ThuNhap.Series["Earnings"].Points.Clear();
            chart_ThuNhap.Series["Earnings"].Points.AddXY("Nam", thunhap_nam);
            chart_ThuNhap.Series["Earnings"].Points.AddXY("Nữ", thunhap_nu);
        }

        private void radio_thunhap_loainhanvien_CheckedChanged(object sender, EventArgs e)
        {
            chart_ThuNhap.Series["Earnings"].Points.Clear();
            chart_ThuNhap.Series["Earnings"].Points.AddXY("Toàn thời gian", thunhap_fulltime);
            chart_ThuNhap.Series["Earnings"].Points.AddXY("Bán thời gian", thunhap_parttime);
        }

        private void tabcontrol_BaoCao_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    load_gv_ThuNhap();
                    break;
                case 1:
                    load_gv_SoNgayNghi();
                    break;
                case 2:
                    load_gv_PhucLoi();
                    break;

            }
        }
    }
}
