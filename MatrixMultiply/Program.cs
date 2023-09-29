using System.Data.Common;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixMultiply
{
    internal class Program
    {
        static void Main(string[] args)
        {            
            int[,] matrix1 = GenerateRandomMatrix(1000, 2000, 2);
            int[,] matrix2 = GenerateRandomMatrix(2000, 3000, 1);
            int[,] matrix3;
            long timeOrigin;

            timeOrigin = TaskMultiple(matrix1, matrix2, out matrix3);
            //Console.WriteLine($"Время расчёта многопоточного (Task) = {timeOrigin} мс");
            //Console.WriteLine(matrix3[0,0]);

            timeOrigin = OriginMultiple(matrix1, matrix2, out matrix3);
            Console.WriteLine($"Время расчёта синхронного = {timeOrigin} мс");
            //Console.WriteLine();
            //Console.WriteLine(matrix3[0, 0]);

            //PrintMatrix(matrix1);
            //Console.WriteLine();
            //PrintMatrix(matrix2);
            //Console.WriteLine();
            //PrintMatrix(matrix3);

            Console.ReadKey();


        }


        static long TaskMultiple(int[,] matrix1, int[,] matrix2, out int[,] matrix_result)
        {
            Stopwatch sw = new();
            sw.Start();

            int rowsAmount = matrix1.GetLength(0);
            int columnsAmount = matrix2.GetLength(1);
            int[,] matrix_result_thread = new int[rowsAmount, columnsAmount];

            // умножение через таски
            Task[] tasks = new Task[rowsAmount * columnsAmount];
            int[] row = new int[matrix1.GetLength(1)];
            int[] column = new int[matrix2.GetLength(0)];

            int[,] matrix2t = new int[matrix2.GetLength(1), matrix2.GetLength(0)];
            for (int i = 0; i < matrix2t.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix2t.GetLength(1); ++j)
                {
                    matrix2t[i, j] = matrix2[j, i];
                }
            }

            for (int i = 0; i < rowsAmount; ++i)
            {
                for (int j = 0; j < columnsAmount; ++j)
                {
                    int iNew = i;
                    int jNew = j;
                    tasks[jNew + iNew * columnsAmount] = Task.Factory.StartNew(() =>
                    {
                        for (int rowInc = 0; rowInc < matrix1.GetLength(1); rowInc++)
                        {
                            row[rowInc] = matrix1[iNew, rowInc];
                        }
                        for (int columnInc = 0; columnInc < matrix2.GetLength(0); columnInc++)
                        {
                            column[columnInc] = matrix2t[jNew, columnInc];
                        }
                        matrix_result_thread[iNew, jNew] = Multiply(row, column);
                    });
                }                
            }
            
            Task.WhenAll(tasks);
            //Task.WaitAll(tasks);
            matrix_result = matrix_result_thread;
            sw.Stop();            
            return sw.ElapsedMilliseconds;

        }

        static int Multiply(int[] row, int[] column)
        {
            int result = 0;
            for (int i = 0; i < row.Length; ++i)
            {
                result += row[i] * column[i];
            }
            return result;
        }

        static long OriginMultiple(int[,] matrix1, int[,] matrix2, out int[,] matrix_result)
        {
            Stopwatch sw = new();
            sw.Start();
            matrix_result = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
            int[,] matrix2t = new int[matrix2.GetLength(1), matrix2.GetLength(0)];
            for (int i = 0; i < matrix2t.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix2t.GetLength(1); ++j)
                {
                    matrix2t[i, j] = matrix2[j, i];
                }
            }
            for (int i = 0; i < matrix1.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix2.GetLength(1); ++j)
                {                    
                    for (int k = 0; k < matrix1.GetLength(1); ++k)
                    {
                        matrix_result[i, j] += matrix1[i, k] * matrix2t[j, k];
                    }

                }
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        static int[,] GenerateRandomMatrix(int rows, int columns, int rndMinMax)
        {
            int[,] matrix = new int[rows, columns];
            //Random rnd = new Random();
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    matrix[i, j] = rndMinMax;// rnd.Next(-rndMinMax, rndMinMax + 1);
                    //Thread.Sleep(1);
                }
                //Thread.Sleep(1);
            }

            return matrix;
        }

        static void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    Console.Write($"{matrix[i,j],3}");
                }
                Console.WriteLine();
            }
        }

    }
}