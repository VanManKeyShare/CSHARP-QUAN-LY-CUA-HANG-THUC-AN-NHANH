﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections;

namespace QUAN_LY_CUA_HANG_THUC_AN_NHANH
{
    public partial class Frm_BA_CHI_TIET : Form
    {
        Class_Funcs CFuncs = new Class_Funcs();

        public string ID_DATA = "";
        public string MODE_CHANGE_DATA = "";
        public string SQL_CONNECTION_STRING = "";

        public Frm_BA_CHI_TIET() { InitializeComponent(); }

        private void btn_thoat_Click(object sender, EventArgs e) { this.Close(); }

        private void Frm_BA_CHI_TIET_Load(object sender, EventArgs e)
        {
            // KIỂM TRA BIẾN ID_DATA ĐƯỢC TRUYỀN ĐẾN. NẾU RỖNG NGHĨA LÀ THÊM MỚI, NGƯỢC LẠI LÀ CẬP NHẬT

            if (ID_DATA.Trim() != "")
            {
                MODE_CHANGE_DATA = "CAP_NHAT";
                XEM_CSDL_THEO_ID(ID_DATA);
                btn_capnhat.Visible = true;
                btn_them.Visible = false;
                txt_ma_ban.Enabled = false;
            }
            else
            {
                MODE_CHANGE_DATA = "THEM_MOI";
                btn_capnhat.Visible = false;
                btn_them.Visible = true;
            }
        }        

        private void XEM_CSDL_THEO_ID(string ID_DATA)
        {
            // LẤY DỮ LIỆU TỪ CSDL

            VMK_DAO_FOR_MS_SQL vmk = new VMK_DAO_FOR_MS_SQL();
            vmk.MS_SQL_CONNECTION_STRING = SQL_CONNECTION_STRING;
            vmk.MS_SQL_QUERY = "SELECT MA_BAN, TEN_BAN FROM BAN_AN WHERE (MA_BAN = @MA_BAN) ORDER BY MA_BAN ASC";
            vmk.MS_SQL_PARAMETERS = vmk.CREATE_MS_SQL_PARAMETERS();
            vmk.MS_SQL_PARAMETERS.Clear();
            vmk.MS_SQL_PARAMETERS.Rows.Add("@MA_BAN", ID_DATA, SqlDbType.VarChar);
            ArrayList KQ = vmk.MS_SQL_SELECT();

            if (KQ[0].ToString() == "ERROR")
            {
                MessageBox.Show(KQ[1].ToString(), "THÔNG BÁO");
                return;
            }

            DataTable DT = (DataTable)KQ[2];

            // SAU ĐÓ NẠP VÀO CÁC CONTROL

            if (DT.Rows.Count != 0)
            {
                txt_ma_ban.Text = DT.Rows[0]["MA_BAN"].ToString().Trim();
                txt_ten_ban.Text = DT.Rows[0]["TEN_BAN"].ToString().Trim();
            }
        }

        private void btn_them_Click(object sender, EventArgs e)
        {
            if (MODE_CHANGE_DATA != "THEM_MOI") { return; }

            // LẤY VÀ KIỂM TRA DỮ LIỆU TỪ FORM

            string ma_ban = txt_ma_ban.Text.Trim().ToUpper();
            string ten_ban = txt_ten_ban.Text.Trim();

            if (ma_ban == "" || ten_ban == "")
            {
                MessageBox.Show("BẠN CHƯA NHẬP ĐẦY ĐỦ DỮ LIỆU", "THÔNG BÁO");

                if (ma_ban == "") { txt_ma_ban.Focus(); return; }
                if (ten_ban == "") { txt_ten_ban.Focus(); return; }

                return;
            }

            if (ma_ban != "" && CFuncs.CHECK_CHARACTER(ma_ban, "_", true, true, false) == false)
            {
                MessageBox.Show("MÃ BÀN ĂN CÓ CHỨA NHỮNG KÝ TỰ KHÔNG HỢP LỆ", "THÔNG BÁO");
                return;
            }

            if (ten_ban != "" && CFuncs.CHECK_CHARACTER(ten_ban, " ", true, true, true) == false)
            {
                MessageBox.Show("TÊN BÀN ĂN CÓ CHỨA NHỮNG KÝ TỰ KHÔNG HỢP LỆ", "THÔNG BÁO");
                return;
            }

            // TIẾN HÀNH THÊM DỮ LIỆU VÀO CSDL

            VMK_DAO_FOR_MS_SQL vmk = new VMK_DAO_FOR_MS_SQL();
            vmk.MS_SQL_CONNECTION_STRING = SQL_CONNECTION_STRING;
            vmk.MS_SQL_QUERY = "INSERT INTO BAN_AN (MA_BAN, TEN_BAN) VALUES (@MA_BAN, @TEN_BAN)";
            vmk.MS_SQL_PARAMETERS = vmk.CREATE_MS_SQL_PARAMETERS();
            vmk.MS_SQL_PARAMETERS.Clear();
            vmk.MS_SQL_PARAMETERS.Rows.Add("@MA_BAN", ma_ban, SqlDbType.VarChar);
            vmk.MS_SQL_PARAMETERS.Rows.Add("@TEN_BAN", ten_ban, SqlDbType.NVarChar);
            String[] KQ = vmk.MS_SQL_INSERT_DELETE_UPDATE();

            if (KQ[0].ToString() == "ERROR")
            {
                Console.WriteLine(KQ[1].ToString());
                if (KQ[1].ToLower().Contains("the insert statement conflicted with the foreign key constraint"))
                {
                    MessageBox.Show("KHÔNG THỂ THÊM. DỮ LIỆU LIÊN KẾT VỚI BẢNG KHÁC KHÔNG TỒN TẠI", "THÔNG BÁO");
                    return;
                }
                if (KQ[1].ToLower().Contains("the update statement conflicted with the foreign key constraint"))
                {
                    MessageBox.Show("KHÔNG THỂ CẬP NHẬT. DỮ LIỆU LIÊN KẾT VỚI BẢNG KHÁC KHÔNG TỒN TẠI", "THÔNG BÁO");
                    return;
                }
                if (KQ[1].ToLower().Contains("cannot insert duplicate key in object"))
                {
                    MessageBox.Show("KHÔNG THỂ THÊM. DỮ LIỆU ĐÃ CÓ RỒI", "THÔNG BÁO");
                    return;
                }
                if (KQ[1].ToLower().Contains("string or binary data would be truncated"))
                {
                    MessageBox.Show("KHÔNG THỂ THÊM. DỮ LIỆU VƯỢT QUÁ QUY ĐỊNH", "THÔNG BÁO");
                    return;
                }
                MessageBox.Show(KQ[1].ToString(), "THÔNG BÁO");
                return;
            }

            MessageBox.Show("THÊM DỮ LIỆU MỚI THÀNH CÔNG", "THÔNG BÁO");

            this.Close();
        }

        private void btn_capnhat_Click(object sender, EventArgs e)
        {
            if (MODE_CHANGE_DATA != "CAP_NHAT") { return; }

            // LẤY VÀ KIỂM TRA DỮ LIỆU TỪ FORM

            string ma_ban = ID_DATA.Trim();
            string ten_ban = txt_ten_ban.Text.Trim();

            if (ma_ban == "" || ten_ban == "")
            {
                MessageBox.Show("BẠN CHƯA NHẬP ĐẦY ĐỦ DỮ LIỆU", "THÔNG BÁO");

                if (ten_ban == "") { txt_ten_ban.Focus(); return; }

                return;
            }

            if (ma_ban != "" && CFuncs.CHECK_CHARACTER(ma_ban, "_", true, true, false) == false)
            {
                MessageBox.Show("MÃ BÀN ĂN CÓ CHỨA NHỮNG KÝ TỰ KHÔNG HỢP LỆ", "THÔNG BÁO");
                return;
            }

            if (ten_ban != "" && CFuncs.CHECK_CHARACTER(ten_ban, " ", true, true, true) == false)
            {
                MessageBox.Show("TÊN BÀN ĂN CÓ CHỨA NHỮNG KÝ TỰ KHÔNG HỢP LỆ", "THÔNG BÁO");
                return;
            }

            // TIẾN HÀNH THÊM DỮ LIỆU VÀO CSDL

            VMK_DAO_FOR_MS_SQL vmk = new VMK_DAO_FOR_MS_SQL();
            vmk.MS_SQL_CONNECTION_STRING = SQL_CONNECTION_STRING;
            vmk.MS_SQL_QUERY = "UPDATE BAN_AN SET TEN_BAN = @TEN_BAN WHERE MA_BAN = @MA_BAN";
            vmk.MS_SQL_PARAMETERS = vmk.CREATE_MS_SQL_PARAMETERS();
            vmk.MS_SQL_PARAMETERS.Clear();
            vmk.MS_SQL_PARAMETERS.Rows.Add("@MA_BAN", ma_ban, SqlDbType.VarChar);
            vmk.MS_SQL_PARAMETERS.Rows.Add("@TEN_BAN", ten_ban, SqlDbType.NVarChar);
            String[] KQ = vmk.MS_SQL_INSERT_DELETE_UPDATE();

            if (KQ[0].ToString() == "ERROR")
            {
                Console.WriteLine(KQ[1].ToString());
                if (KQ[1].ToLower().Contains("the insert statement conflicted with the foreign key constraint"))
                {
                    MessageBox.Show("KHÔNG THỂ THÊM. DỮ LIỆU LIÊN KẾT VỚI BẢNG KHÁC KHÔNG TỒN TẠI", "THÔNG BÁO");
                    return;
                }
                if (KQ[1].ToLower().Contains("the update statement conflicted with the foreign key constraint"))
                {
                    MessageBox.Show("KHÔNG THỂ CẬP NHẬT. DỮ LIỆU LIÊN KẾT VỚI BẢNG KHÁC KHÔNG TỒN TẠI", "THÔNG BÁO");
                    return;
                }
                if (KQ[1].ToLower().Contains("cannot insert duplicate key in object"))
                {
                    MessageBox.Show("KHÔNG THỂ THÊM. DỮ LIỆU ĐÃ CÓ RỒI", "THÔNG BÁO");
                    return;
                }
                if (KQ[1].ToLower().Contains("string or binary data would be truncated"))
                {
                    MessageBox.Show("KHÔNG THỂ THÊM. DỮ LIỆU VƯỢT QUÁ QUY ĐỊNH", "THÔNG BÁO");
                    return;
                }
                MessageBox.Show(KQ[1].ToString(), "THÔNG BÁO");
                return;
            }

            MessageBox.Show("CẬP NHẬT DỮ LIỆU THÀNH CÔNG", "THÔNG BÁO");

            this.Close();
        }
    }
}