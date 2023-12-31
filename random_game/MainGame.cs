﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace random_game {
    public partial class MainGame : Form {
        private readonly Button[] _buttons = new Button[6];
        private readonly Color[] _initialColors = new Color[6];
        private const int initialTokenCount = 100;
        private bool _finishRoll = true;
        private List<Button> _selectedButtons = new List<Button>();
        public static readonly int[] RollInt = new int[3];

        public int token {
            get => getTokenCount();
            set => _updateTokenTxt(value.ToString());
        }

        public MainGame() {
            InitializeComponent();

            Button[] buttons = { btn1, btn2, btn3, btn4, btn5, btn6 };
            for (var i = 0; i < buttons.Length; i++) {
                _buttons[i] = buttons[i];
                _initialColors[i] = buttons[i].BackColor;
            }

            token = initialTokenCount;
        }

        private void roll() {
            var rolls = updateRolls();
            RollInt[0] = int.Parse(rolls.Split(' ')[0]);
            RollInt[1] = int.Parse(rolls.Split(' ')[1]);
            RollInt[2] = int.Parse(rolls.Split(' ')[2]);

            updateRollTxt(rolls);
            highlightBtn(RollInt);
            updateScoreTxt(calcScore(RollInt).ToString());

            if (RollInt[0] == RollInt[1] && RollInt[1] == RollInt[2]) blinkBtn(RollInt[0] - 1);

            token = getTokenCount() + calcScore(RollInt);
            _finishRoll = true;
        }

        private void btn_MouseUp(object sender, MouseEventArgs e) {
            var btn = (Button)sender;
            var currentNumber = getBtnInt(btn);
            var increment = e.Button == MouseButtons.Left ? 1 : -1;
            if (getTokenCount() == 0 && increment == 1) increment = 0;

            if (_finishRoll == false) {
                var newValue = currentNumber + increment;

                if (_selectedButtons.Count >= 3 && !_selectedButtons.Contains(btn)) return;

                if (newValue >= 0) token -= increment;
                if (newValue < 0) newValue = 0;
                if (newValue == 0) {
                    setBtnTxt(btn, "");
                    _selectedButtons.Remove(btn);
                } else {
                    setBtnTxt(btn, newValue.ToString());
                    if (!_selectedButtons.Contains(btn)) _selectedButtons.Add(btn);
                }
            } else if (_finishRoll) {
                resetBtns();
                _finishRoll = false;
                token = getTokenCount();
                if (getTokenCount() == 0) {
                    updateRollTxt(@"No tokens");
                    return;
                }

                btn_MouseUp(sender, e);
            }
        }

        private void btn_MouseWheel(object sender, MouseEventArgs e) {
            btn_MouseUp(sender, new MouseEventArgs(e.Delta > 0 ? MouseButtons.Left : MouseButtons.Right, 1, 0, 0, 0));
        }

        private void btnSpin_Click(object sender, EventArgs e) {
            if (_finishRoll || _selectedButtons.Count == 0) return;
            roll();
            using (var start = new SpinForm()) {
                Hide();
                start.ShowDialog();
            }

            Show();
        }

        private void updateRollTxt(string s) {
            btnSpin.Text = s;
        }

        private static void setBtnTxt(Control btn, string s) {
            btn.Text = s;
        }

        private static string getBtnTxt(Control btn) {
            return btn.Text;
        }

        private static int getBtnInt(Control btn) {
            return int.TryParse(getBtnTxt(btn), out var i) ? i : 0;
        }

        private void _updateTokenTxt(string s) {
            var text = @"Tokens: " + tokenScale(s);
            txtTokensCount.Text = text;
        }

        private int getTokenCount() {
            var rePattern = new Regex(@"(?<=:\s)*\d+");
            return int.TryParse(rePattern.Match(txtTokensCount.Text).Value, out var tokenCount)
                ? tokenCount
                : initialTokenCount;
        }

        private void updateScoreTxt(string s) {
            var text = @"Your score: " + tokenScale(s);
            txtScore.Text = text;
        }

        private static string updateRolls() {
            var rnd = new Random();
            var randomNumber1 = rnd.Next(1, 7);
            var randomNumber2 = rnd.Next(1, 7);
            var randomNumber3 = rnd.Next(1, 7);
            return randomNumber1 + @" " + randomNumber2 + @" " + randomNumber3;
        }

        private void resetBtns() {
            for (var i = 0; i < _buttons.Length; i++) {
                setBtnTxt(_buttons[i], "");
                setBtnColor(_buttons[i], _initialColors[i]);
            }

            _selectedButtons = new List<Button>();
        }

        private int calcScore(IReadOnlyList<int> scores) {
            var score = 0;
            for (var i = 0; i < _buttons.Length; i++)
                if (scores[0] == i + 1 && scores[1] == i + 1 && scores[2] == i + 1) {
                    score += getBtnInt(_buttons[i]) * 4;
                } else if (scores[0] == i + 1 && scores[1] == i + 1 || scores[1] == i + 1 && scores[2] == i + 1 ||
                           scores[0] == i + 1 && scores[2] == i + 1) {
                    score += getBtnInt(_buttons[i]) * 3;
                } else {
                    if (scores[0] == i + 1 || scores[1] == i + 1 || scores[2] == i + 1)
                        score += getBtnInt(_buttons[i]) * 2;
                }

            return score;
        }

        private static void setBtnColor(Control btn, Color color) {
            btn.BackColor = color;
        }

        private void highlightBtn(IReadOnlyList<int> indexes) {
            for (var i = 0; i < _buttons.Length; i++)
                if (i != indexes[0] - 1 && i != indexes[1] - 1 && i != indexes[2] - 1)
                    setBtnColor(_buttons[i], Color.Gray);
                else
                    setBtnColor(_buttons[i], _initialColors[i]);
        }

        private void blinkBtn(int index) {
            var blinkTimer = new Timer();
            blinkTimer.Interval = 500;
            var ticks = 10;
            blinkTimer.Tick += (sender, args) => {
                if (ticks <= 0) {
                    blinkTimer.Stop();
                    setBtnColor(_buttons[index], _initialColors[index]);
                } else {
                    setBtnColor(_buttons[index],
                        _buttons[index].BackColor == Color.Gray ? _initialColors[index] : Color.Gray);
                    ticks--;
                }
            };
            blinkTimer.Start();
        }

        private static string tokenScale(string token) {
            int.TryParse(token, out var nToken);
            var count = 0;
            while (nToken >= 1000) {
                token = (nToken / 1000).ToString();
                nToken /= 1000;
                count++;
            }

            switch (count) {
                case 1:
                    token += "K";
                    break;
                case 2:
                    token += "M";
                    break;
                case 3:
                    token += "B";
                    break;
                case 4:
                    token += "T";
                    break;
            }

            return token;
        }

        private void txtTokensCount_Click(object sender, EventArgs e) {
            using (var start = new TokenMenu(this)) {
                start.ShowDialog();
            }
        }

        private void txtTokensCount_MouseHover(object sender, EventArgs e) {
            txtTokensCount.Image = Properties.Resources.iconAdd;
        }

        private void txtTokensCount_MouseLeave(object sender, EventArgs e) {
            txtTokensCount.Image = Properties.Resources.iconToken;
        }
    }
}