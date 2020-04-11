using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageLab {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e) {
            float[,] kernel = MathMorfology.Kernel;
            dataGridView1.RowCount = kernel.GetLength(0);
            dataGridView1.ColumnCount = kernel.GetLength(1);
            for(int i = 0; i < dataGridView1.RowCount; i++) {
                for(int j = 0; j < dataGridView1.ColumnCount; j++) {
                    //dataGridView1.Rows[i].Cells[j].Value = kernel[i, j];
                    if(kernel[i, j] == 1) {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
                    else {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                }
            }
            foreach(DataGridViewColumn c in dataGridView1.Columns) {
                c.Width = dataGridView1.Rows[1].Height;
                c.Resizable = DataGridViewTriState.False;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            dataGridView1.ColumnCount = Convert.ToInt32(numericUpDown1.Value);
            dataGridView1.RowCount = Convert.ToInt32(numericUpDown1.Value);
            for (int i = 0; i < dataGridView1.RowCount; i++) {
                for (int j = 0; j < dataGridView1.ColumnCount; j++) {
                    if(dataGridView1.Rows[i].Cells[j].Value == null) {
                        //dataGridView1.Rows[i].Cells[j].Value = 0;
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                }
            }
            foreach (DataGridViewColumn c in dataGridView1.Columns) {
                c.Width = dataGridView1.Rows[0].Height;
                c.Resizable = DataGridViewTriState.False;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.White) {
                //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 1;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Black;
            }
            else {
                //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            float[,] kernel = new float[dataGridView1.RowCount, dataGridView1.ColumnCount];
            for(int i = 0; i < dataGridView1.RowCount; i++) {
                for(int j = 0; j < dataGridView1.ColumnCount; j++) {
                    if(dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Black) {
                        kernel[i, j] = 1;
                    }
                    else {
                        kernel[i, j] = 0;
                    }
                }
            }
            MathMorfology.createMathMorfKernel(kernel);
            Close();
        }
    }
}
