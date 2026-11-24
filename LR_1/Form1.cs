using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LR_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Зчитування дійсного числа: приймає і кому, і крапку як роздільник
        private double ReadDouble(TextBox tb, string name)
        {
            string s = tb.Text.Trim().Replace(',', '.');
            double value;
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                throw new FormatException("Некоректне значення поля \"" + name + "\"");
            return value;
        }

        // f1(x), якщо x <= 0
        private double F1(double x)
        {
            return Math.Cos(x * x * x - 4 * x + 4) / (x * x * x - Math.Log(Math.Abs(x) + 1));
        }

        // f2(x), якщо 0 < x <= a
        private double F2(double x)
        {
            return Math.Pow(Math.Sin(x + 2), 2) / Math.Pow(2 * x * x + Math.Pow(x, 4) + 1, 1.0 / 3);
        }

        // f3(x), якщо x > a
        private double F3(double x)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(x), 3)) * Math.Sin(x * x * x) / Math.Pow(Math.Cos(x + 1), 2);
        }

        // Кусково-задана функція y = f(x)
        private double F(double x, double a, out string branch)
        {
            if (x <= 0)
            {
                branch = "f1";
                return F1(x);
            }
            else if (x <= a)
            {
                branch = "f2";
                return F2(x);
            }
            else
            {
                branch = "f3";
                return F3(x);
            }
        }

        private static bool IsValid(double y)
        {
            return !double.IsNaN(y) && !double.IsInfinity(y);
        }

        // Табулювання функції на відрізку [xn; xk] з кроком h
        private void button1_Click(object sender, EventArgs e)
        {
            double xn, xk, h, a, x, y;
            string branch;
            try
            {
                xn = ReadDouble(textBox1, "xn");
                xk = ReadDouble(textBox2, "xk");
                h = ReadDouble(textBox3, "h");
                a = ReadDouble(textBox4, "a");

                if (h <= 0)
                {
                    MessageBox.Show("Крок h повинен бути більшим за нуль", "Помилка");
                    return;
                }
                if (xn > xk)
                {
                    MessageBox.Show("Початкове значення xn не може бути більшим за xk", "Помилка");
                    return;
                }

                dg.Rows.Clear();
                chart1.Series[0].Points.Clear();

                // Кількість кроків рахуємо заздалегідь, щоб уникнути накопичення похибки
                int n = (int)Math.Floor((xk - xn) / h + 1e-9);
                for (int i = 0; i <= n; i++)
                {
                    x = Math.Round(xn + i * h, 6);
                    y = F(x, a, out branch);

                    if (IsValid(y))
                    {
                        dg.Rows.Add(x.ToString(), Math.Round(y, 6).ToString(), branch);
                        chart1.Series[0].Points.AddXY(x, y);
                    }
                    else
                    {
                        dg.Rows.Add(x.ToString(), "не визначено", branch);
                    }
                }
                labelCount.Text = "Кількість точок: " + (n + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка введення даних!\n" + ex.Message, "Помилка");
            }
        }

        // Обчислення y для одного введеного значення x
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                double a = ReadDouble(textBox4, "a");
                double x = ReadDouble(textBox5, "x");
                string branch;
                double y = F(x, a, out branch);

                if (IsValid(y))
                    textBox6.Text = Math.Round(y, 6).ToString() + "   (" + branch + ")";
                else
                    textBox6.Text = "не визначено (" + branch + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка введення даних!\n" + ex.Message, "Помилка");
            }
        }

        // Очищення таблиці, графіка та результату
        private void button3_Click(object sender, EventArgs e)
        {
            dg.Rows.Clear();
            chart1.Series[0].Points.Clear();
            textBox5.Clear();
            textBox6.Clear();
            labelCount.Text = "Кількість точок: 0";
        }
    }
}
