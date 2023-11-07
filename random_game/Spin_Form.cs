﻿using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace random_game {
    public partial class SpinForm : Form {
        private const int PictureBoxWidth = 120; // Độ rộng mong muốn của mỗi pictureBox
        private const int PictureBoxHeight = 120; // Độ cao mong muốn của mỗi pictureBox
        private const int PictureBoxCount = 3;

        public SpinForm() {
            InitializeComponent();
            InitializeMediaPlayer();
            Hide_All();
        }

        public int[] RollInt { get; private set; }

        private void InitializeMediaPlayer() {
            rollAnimation.URL = @".\Resources\roll.mp4";
            rollAnimation.PlayStateChange += AxWindowsMediaPlayer1_PlayStateChange;
            rollAnimation.ClickEvent += Roll_animation_ClickEvent;
            rollAnimation.uiMode = "none";
        }

        private void Hide_All() {
            roll1.Hide();
            roll2.Hide();
            roll3.Hide();
        }

        private void Show_All() {
            roll1.Show();
            roll2.Show();
            roll3.Show();
            var spacing = 10;
            // Tính toán tổng độ rộng của tất cả pictureBox và khoảng cách giữa chúng
            var totalWidth = (PictureBoxWidth + spacing) * PictureBoxCount - spacing;
            // Tính toán vị trí bắt đầu của hàng ngang để đảm bảo nó giữa form
            var startX = (ClientSize.Width - totalWidth) / 2;
            // Tính toán vị trí y bắt đầu để đảm bảo nó giữa form
            var startY = (ClientSize.Height - PictureBoxHeight) / 2;
            // Cấu hình kích thước và vị trí của từng pictureBox
            roll1.Size = new Size(PictureBoxWidth, PictureBoxHeight);
            roll1.Location = new Point(startX, startY);
            roll2.Size = new Size(PictureBoxWidth, PictureBoxHeight);
            roll2.Location = new Point(startX + PictureBoxWidth + spacing, startY);
            roll3.Size = new Size(PictureBoxWidth, PictureBoxHeight);
            roll3.Location = new Point(startX + 2 * (PictureBoxWidth + spacing), startY);
        }

        private void AxWindowsMediaPlayer1_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e) {
            if (rollAnimation.playState == WMPPlayState.wmppsMediaEnded ||
                rollAnimation.playState == WMPPlayState.wmppsStopped) {
                rollAnimation.Hide();
                RollInt = MainGame.rollInt;
                LoadImageIntoPictureBox();
                Show_All();
            }
        }

        private void LoadImageIntoPictureBox() {
            string[] arrayUrl = {
                @".\Resources\wanderer.png", @".\Resources\neuvillete.png",
                @".\Resources\zongli.png", @".\Resources\raiden.png",
                @".\Resources\nilou.png", @".\Resources\kokomi.png"
            };

            LoadImageIntoPictureBoxHelper(arrayUrl[0], roll1);
            LoadImageIntoPictureBoxHelper(arrayUrl[1], roll2);
            LoadImageIntoPictureBoxHelper(arrayUrl[2], roll3);
        }

        private void LoadImageIntoPictureBoxHelper(string imagePath, PictureBox pictureBox) {
            // Kiểm tra xem tập tin hình ảnh có tồn tại không
            if (System.IO.File.Exists(imagePath)) {
                // Tạo một đối tượng hình ảnh từ đường dẫn
                var image = Image.FromFile(imagePath);
                // Đặt hình ảnh cho PictureBox
                pictureBox.Image = image;
                // Cấu hình PictureBox để hiển thị hình ảnh theo tỷ lệ
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                // Cập nhật kích thước của PictureBox theo kích thước của hình ảnh
                pictureBox.Size = image.Size;
                // Cập nhật tên tập tin hình ảnh nếu bạn muốn
                pictureBox.Tag = imagePath;
            } else {
                MessageBox.Show(@"File not found: " + imagePath);
            }
        }

        private void roll_Click(object sender, EventArgs e) {
            Close();
        }

        private void Roll_animation_ClickEvent(object sender, _WMPOCXEvents_ClickEvent e) {
            rollAnimation.Ctlcontrols.stop();
        }
    }
}