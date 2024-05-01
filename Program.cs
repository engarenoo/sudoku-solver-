using Microsoft.Z3;



class Program

{

    static void Main(string[] args)

    {

        // ایجاد یک زمینه محدودیت با استفاده از کتابخانه Z3

        using (Context ctx = new Context())

        {

            // 9x9 ماتریس از متغیرهای عدد صحیح

            IntExpr[][] X = new IntExpr[9][];

            for (int i = 0; i < 9; i++)

            {

                X[i] = new IntExpr[9];

                for (int j = 0; j < 9; j++)

                {

                    X[i][j] = ctx.MkIntConst($"X_{i}_{j}");

                }

            }



            // افزودن محدودیت‌هایی که هر خانه شامل یک عدد بین 1 تا 9 باشد

            Solver s = ctx.MkSolver();

            for (int i = 0; i < 9; i++)

            {

                for (int j = 0; j < 9; j++)

                {

                    s.Add(ctx.MkAnd(ctx.MkLe(ctx.MkInt(1), X[i][j]), ctx.MkLe(X[i][j], ctx.MkInt(9))));

                }

            }



            // افزودن محدودیت‌هایی که هر سطر فقط یک بار هر عدد را شامل کند

            for (int i = 0; i < 9; i++)

            {

                s.Add(ctx.MkDistinct(X[i]));

            }



            // افزودن محدودیت‌هایی که هر ستون فقط یک بار هر عدد را شامل کند

            for (int j = 0; j < 9; j++)

            {

                IntExpr[] column = new IntExpr[9];

                for (int i = 0; i < 9; i++)

                {

                    column[i] = X[i][j];

                }

                s.Add(ctx.MkDistinct(column));

            }



            // افزودن محدودیت‌هایی که هر مربع 3x3 فقط یک بار هر عدد را شامل کند

            for (int i0 = 0; i0 < 3; i0++)

            {

                for (int j0 = 0; j0 < 3; j0++)

                {

                    IntExpr[] square = new IntExpr[9];

                    for (int i = 0; i < 3; i++)

                    {

                        for (int j = 0; j < 3; j++)

                        {

                            square[3 * i + j] = X[3 * i0 + i][3 * j0 + j];

                        }

                    }

                    s.Add(ctx.MkDistinct(square));

                }

            }



            // تعیین نمونه سودوکو، عدد 0 به معنی خانه‌ی خالی است

            int[,] instance =

                {{0, 0, 0, 0, 6, 1, 0, 0, 2},

                 {0, 7, 0, 0, 0, 0, 0, 6, 0},

                 {9, 2, 0, 0, 0, 0, 0, 0, 0},

                 {0, 0, 4, 5, 2, 0, 9, 0, 0},

                 {0, 8, 2, 1, 0, 4, 6, 3, 0},

                 {0, 0, 3, 0, 7, 6, 1, 0, 0},

                 {0, 0, 0, 0, 0, 0, 0, 9, 8},

                 {0, 3, 0, 0, 0, 0, 0, 4, 0},

                 {6, 0, 0, 3, 8, 0, 0, 0, 0}};



            // افزودن محدودیت‌های نمونه سودوکو به زمینه

            for (int i = 0; i < 9; i++)

            {

                for (int j = 0; j < 9; j++)

                {

                    if (instance[i, j] != 0)

                    {

                        s.Add(ctx.MkEq(X[i][j], ctx.MkInt(instance[i, j])));

                    }

                }

            }



            // بررسی وجود راه حل

            if (s.Check() == Status.SATISFIABLE)

            {

                // در صورت وجود راه حل، چاپ آن

                Model m = s.Model;

                for (int i = 0; i < 9; i++)

                {

                    for (int j = 0; j < 9; j++)

                    {

                        Console.Write($"{m.Evaluate(X[i][j])} ");

                    }

                    Console.WriteLine();

                }

            }

        }

    }

}

