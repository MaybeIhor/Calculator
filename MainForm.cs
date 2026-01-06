using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Calculus
{
    public partial class form : Form
    {
        [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] val, int size);

        private static readonly CultureInfo EnUs = CultureInfo.GetCultureInfo("en-US");
        private static bool isRad = true;
        private static bool isInverse = false;
        private bool dark = false;
        private string prev = null;

        public form()
        {
            InitializeComponent();
            ApplySystemTheme();
            toolStrip.Renderer = new FixedRenderer();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        private bool IsDarkModeEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
            {
                return key?.GetValue("AppsUseLightTheme") is object val && int.Parse(val.ToString()) == 0;
            }
        }

        private void ApplySystemTheme()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
            {
                if (key?.GetValue("AppsUseLightTheme") is int theme && theme == 0)
                {
                    DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
                    Color darkBg = Color.FromArgb(255, 25, 25, 25);
                    Color darkerBg = Color.FromArgb(255, 19, 19, 19);

                    toolStrip.BackColor = darkBg;
                    inputBox.BackColor = BackColor = outputBox.BackColor = darkerBg;
                    toolStrip.ForeColor = inputBox.ForeColor = SystemColors.Window;
                    invLabel.ForeColor = radLabel.ForeColor = histLabel.ForeColor = outputBox.ForeColor = SystemColors.ControlDark;
                    dark = true;
                }
            }
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            if (inputBox.Text.Length == 0) return;

            int pos = inputBox.SelectionStart;
            int len = inputBox.SelectionLength;

            if (len > 0)
            {
                inputBox.Text = inputBox.Text.Remove(pos, len);
                inputBox.SelectionStart = pos;
            }
            else if (pos > 0)
            {
                inputBox.Text = inputBox.Text.Remove(pos - 1, 1);
                inputBox.SelectionStart = pos - 1;
            }

            inputBox.Focus();
        }

        private void ToolStripButton2_Click(object sender, EventArgs e) => inputBox.Clear();
        private void ToolStripButton3_Click(object sender, EventArgs e) => ShowPlot();

        private void ShowPlot()
        {
            var plot = new plotForm(dark);
            plot.Show();
        }

        private void InsertText(string text) => inputBox.SelectedText = text;

        private void ToolStripButton4_Click(object sender, EventArgs e) => InsertText("(");
        private void ToolStripButton5_Click(object sender, EventArgs e) => InsertText(")");
        private void ToolStripButton6_Click(object sender, EventArgs e) => InsertText("7");
        private void ToolStripButton7_Click(object sender, EventArgs e) => InsertText("8");
        private void ToolStripButton8_Click(object sender, EventArgs e) => InsertText("9");
        private void ToolStripButton9_Click(object sender, EventArgs e) => InsertText("rt(2,");
        private void ToolStripButton10_Click(object sender, EventArgs e) => InsertText("^");
        private void ToolStripButton11_Click(object sender, EventArgs e) => InsertText("4");
        private void ToolStripButton12_Click(object sender, EventArgs e) => InsertText("5");
        private void ToolStripButton13_Click(object sender, EventArgs e) => InsertText("6");
        private void ToolStripButton14_Click(object sender, EventArgs e) => InsertText("*");
        private void ToolStripButton15_Click(object sender, EventArgs e) => InsertText("/");
        private void ToolStripButton16_Click(object sender, EventArgs e) => InsertText("1");
        private void ToolStripButton17_Click(object sender, EventArgs e) => InsertText("2");
        private void ToolStripButton18_Click(object sender, EventArgs e) => InsertText("3");
        private void ToolStripButton19_Click(object sender, EventArgs e) => InsertText("-");
        private void ToolStripButton20_Click(object sender, EventArgs e) => InsertText("+");
        private void ToolStripButton21_Click(object sender, EventArgs e) => InsertText("0");
        private void ToolStripButton22_Click(object sender, EventArgs e) => InsertText(".");
        private void ToolStripButton23_Click(object sender, EventArgs e) => InsertText("π");
        private void ToolStripButton25_Click(object sender, EventArgs e) => InsertText(isInverse ? "asin(" : "sin(");
        private void ToolStripButton26_Click(object sender, EventArgs e) => InsertText(isInverse ? "acos(" : "cos(");
        private void ToolStripButton27_Click(object sender, EventArgs e) => InsertText(isInverse ? "atan(" : "tan(");
        private void ToolStripButton28_Click(object sender, EventArgs e) => InsertText("log(10,");
        private void ToolStripButton24_Click(object sender, EventArgs e)
        {
            prev = inputBox.Text;
            inputBox.Text = outputBox.Text;
        }

        private void FactButton_Click(object sender, EventArgs e) => InsertText("!");

        public static double Evaluate(string ex)
        {
            var operands = new Stack<double>();
            var operators = new Stack<string>();
            int i = 0;
            bool expectOperand = true;

            while (i < ex.Length)
            {
                if (char.IsWhiteSpace(ex[i])) { i++; continue; }

                if (char.IsDigit(ex[i]) || ex[i] == '.')
                {
                    int start = i;
                    while (i < ex.Length && (char.IsDigit(ex[i]) || ex[i] == '.' || ex[i] == 'e'||
                           (i > start && (ex[i] == '+' || ex[i] == '-') && (ex[i - 1] == 'e'))))
                        i++;

                    operands.Push(double.Parse(ex.Substring(start, i - start), NumberStyles.Float, EnUs));
                    expectOperand = false;
                }
                else if (ex[i] == 'e' && (i == 0 || !char.IsDigit(ex[i - 1])))
                {
                    operands.Push(Math.E);
                    i++;
                    expectOperand = false;
                }
                else if (ex[i] == 'π' && (i == 0 || !char.IsDigit(ex[i - 1])))
                {
                    operands.Push(Math.PI);
                    i++;
                    expectOperand = false;
                }
                else if (ex[i] == '!')
                {
                    double val = operands.Pop();
                    operands.Push(Factorial(val));
                    i++;
                    expectOperand = false;
                }
                else if (ex[i] == '-' && expectOperand)
                {
                    operands.Push(0);
                    operators.Push("-");
                    i++;
                }
                else if (char.IsLetter(ex[i]))
                {
                    int start = i;
                    while (i < ex.Length && char.IsLetter(ex[i])) i++;
                    operators.Push(ex.Substring(start, i - start));
                    expectOperand = true;
                }
                else if (ex[i] == '(')
                {
                    operators.Push("(");
                    i++;
                    expectOperand = true;
                }
                else if (ex[i] == ',')
                {
                    while (operators.Peek() != "(") Apply(operands, operators.Pop());
                    i++;
                    expectOperand = true;
                }
                else if (ex[i] == ')')
                {
                    while (operators.Peek() != "(") Apply(operands, operators.Pop());
                    operators.Pop();
                    if (operators.Count > 0 && IsFunction(operators.Peek()))
                        Apply(operands, operators.Pop());
                    i++;
                    expectOperand = false;
                }
                else if (IsOperator(ex[i]))
                {
                    string op = ex[i].ToString();
                    while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(op))
                        Apply(operands, operators.Pop());
                    operators.Push(op);
                    i++;
                    expectOperand = true;
                }
                else throw new Exception();
            }

            while (operators.Count > 0) Apply(operands, operators.Pop());
            return operands.Pop();
        }

        private static double Factorial(double n)
        {
            if (n == 0 || n == 1) return 1;

            double result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;

            return result;
        }

        private static bool IsOperator(char c) => "+-*/^".Contains(c);
        private static bool IsFunction(string op) => op == "sin" || op == "cos" || op == "tan" || op == "asin" || op == "acos" || op == "atan" || op == "log" || op == "rt";

        private static void Apply(Stack<double> operands, string op)
        {
            if (op == "sin" || op == "cos" || op == "tan")
            {
                double radians = isRad ? operands.Pop() : (operands.Pop() * Math.PI / 180.0);
                operands.Push(op == "sin" ? Math.Sin(radians) : op == "cos" ? Math.Cos(radians) : Math.Tan(radians));
                return;
            }

            if (op == "asin" || op == "acos" || op == "atan")
            {
                double value = operands.Pop();
                double result = op == "asin" ? Math.Asin(value) : op == "acos" ? Math.Acos(value) : Math.Atan(value);
                operands.Push(isRad ? result : (result * 180.0 / Math.PI));
                return;
            }

            double b = operands.Pop();
            double a = operands.Pop();

            switch (op)
            {
                case "+": operands.Push(a + b); break;
                case "-": operands.Push(a - b); break;
                case "*": operands.Push(a * b); break;
                case "/": operands.Push(a / b); break;
                case "^": operands.Push(Math.Pow(a, b)); break;
                case "log": operands.Push(Math.Log(b, a)); break;
                case "rt": operands.Push(Math.Pow(b, 1 / a)); break;
            }
        }

        private static int Precedence(string op)
        {
            switch (op)
            {
                case "+":
                case "-": return 1;
                case "*":
                case "/": return 2;
                case "^": return 3;
                case "sin":
                case "cos":
                case "tan":
                case "asin":
                case "acos":
                case "atan":
                case "log": return 4;
                default: return 0;
            }
        }

        private bool Solve()
        {
            string func = inputBox.Text.Replace(" ", string.Empty);

            try
            {
                outputBox.Text = Evaluate(func).ToString(EnUs);
            }
            catch
            {
                inputBox.ForeColor = Color.FromArgb(156, 40, 40);
                return false;
            }
            return true;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Solve();
            }
        }

        private void InputBox_TextChanged(object sender, EventArgs e)
        {
            if (Solve())
            {
                if (IsDarkModeEnabled())
                    inputBox.ForeColor = SystemColors.Window;
                else
                    inputBox.ForeColor = SystemColors.WindowText;
            }
        }

        private void radLabel_Click(object sender, EventArgs e)
        {
            isRad = !isRad;
            radLabel.Text = isRad ? "rad" : "deg";
            Solve();
        }

        private void invLabel_Click(object sender, EventArgs e)
        {
            isInverse = !isInverse;
            invLabel.Text = isInverse ? "inv" : "std";
        }

        private void histLabel_Click(object sender, EventArgs e)
        {
            if (prev != null)
                inputBox.Text = prev;
        }
    }
}