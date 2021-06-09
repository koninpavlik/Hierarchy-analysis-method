using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mapack;

namespace Lab4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            fillGrid();
        }

        private void fillGrid()
        {
            for (int i = 0; i < numericUpDownElements.Value; i++)
            {
                dataGridView1.Columns.Add(i.ToString(), i.ToString());
                dataGridView1.Columns[i].DefaultCellStyle.NullValue = "0";
            }
            for (int i = 0; i < numericUpDownCriteries.Value; i++)
            {
                dataGridView1.Rows.Add();
            }
        }

        private void numericUpDownCriteries_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(((UpDownBase)sender).Text) < numericUpDownCriteries.Value)
            {
                dataGridView1.Rows.Add();
            }
            else
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
            }
        }

        private void numericUpDownElements_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(((UpDownBase)sender).Text) < numericUpDownElements.Value)
            {
                dataGridView1.Columns.Add((dataGridView1.Columns.Count).ToString(), (dataGridView1.Columns.Count).ToString());
                dataGridView1.Columns[dataGridView1.Columns.Count - 1].DefaultCellStyle.NullValue = "0";
            }
            else
            {
                dataGridView1.Columns.Remove((Convert.ToInt32(((UpDownBase)sender).Text) - 1).ToString());
            }
        }

        private void buttonRandomize_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    dataGridView1.Rows[j].Cells[i].Value = rnd.Next(0, 3).ToString();
                }
            }
        }

        private static double GetSumOfVector(List<double> vector)
        {
            double sum = 0;
            foreach (var element in vector)
            {
                sum += element;
            }
            return sum;
        }

        private static Matrix GetMatrixFromListOfLists(List<List<double>> matrix)
        {
            var mtrx = new Matrix(matrix.Count, matrix[0].Count);

            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    mtrx[i, j] = Math.Round(matrix[i][j], 2);
                }
            }

            return mtrx;
        }

        private static string MatrixToString(Matrix matrix)
        {
            string result = "";
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    result += matrix[i, j].ToString("0.00") + "  ";
                }
                result += "\n";
            }
            result += "\n";
            return result;
        }

        private static List<List<double>> dgwTo2dListOfDouble(DataGridView dataGridView)
        {
            List<List<double>> list = new List<List<double>>();
            for (int i2 = 0; i2 < dataGridView.Rows.Count; i2++)
            {
                list.Add(new List<double>());
                for (int j2 = 0; j2 < dataGridView.Columns.Count; j2++)
                {
                    list[i2].Add(Convert.ToDouble(dataGridView.Rows[i2].Cells[j2].Value));
                }
            }
            return list;
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                List<List<double>> matrix = dgwTo2dListOfDouble(dataGridView1);

                Matrix A = GetMatrixFromListOfLists(matrix);

                var eigen = new EigenvalueDecomposition(A);
                double Lmax = 0;
                richTextBox1.Text = "Корни характеристического уравнения: \n";
                foreach (var item in eigen.RealEigenvalues)
                {
                    richTextBox1.Text += item.ToString("0.00") + "\n";
                    Lmax = item > Lmax ? item : Lmax;
                }

                richTextBox1.Text += "Максимальный корень из найденных: " + Lmax.ToString("0.00") + "\n";
                var CI = (Lmax - A.Columns) / (A.Columns - 1);
                richTextBox1.Text += "Индекс совместности: " + CI.ToString("0.00") + "\n";
                if (CI > 0.1)
                {
                    richTextBox1.Text += "Ошибка: Индекс совместности системы > 0.1" + "\n";
                    return;
                }

                // новое значение диагонального элемента матрицы
                for (int d = 0; d < A.Columns; d++)
                {
                    A[d, d] = 1 - Lmax;
                }

                richTextBox1.Text += "Полученная матрица для составления однородной СЛАУ \n";

                richTextBox1.Text += MatrixToString(A);

                // решить систему уравнений
                var B = new Matrix(A.Rows, 1);
                for (int i = 0; i < A.Rows; i++)
                {
                    B[i, 0] = Math.Pow(0.1, 2);
                }

                // нормировать полученные значения
                var W = A.Solve(B);

                var summa = 0.0;

                for (int i = 0; i < A.Rows; i++)
                {
                    summa += Math.Round(W[i, 0], 2);
                }

                for (int i = 0; i < A.Rows; i++)
                {
                    W[i, 0] = Math.Round(W[i, 0] / summa, 2);
                }

                richTextBox1.Text += "Искомый нормированный весовой вектор: \n";
                richTextBox1.Text += MatrixToString(W);
            }
            catch (Exception)
            {
                richTextBox1.Text = "Вводить нужно квадратную матрицу";
            }

        }
    }
}
