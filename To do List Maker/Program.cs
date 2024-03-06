using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading;

namespace To_do_List_Maker
{
    internal class Program
    {
        static void Main(string[] _)
        {
            Console.SetWindowSize(127, 30); Console.SetBufferSize(200, 200); Console.SetWindowPosition(70, 100); // startup conditions for the console

            List<Account> accounts = new List<Account>(); // creates a list of objects from the abstract class account

        Main: // TODO: TEXT TO SPEECH INTRO, DOES NOT ACCEPT EMPTY SIGNUP, TASK SORTER
            PlaySuccess(); // startup sound
            int menu = TitleMenu(); // takes the choice number that was inputted after the title menu was shown

            switch (menu)
            {
                case 0: // exit the application
                    Console.Clear();
                    PlaySuccess(); EndScreen();
                    Thread.Sleep(1500);
                    Environment.Exit(0);
                    break;
                case 1: // signup
                    string name = default, uname = default, pass = default;
                    int unamechecker = 0;

                    Console.Clear();
                    Signup();
                    SignupMenu(ref name, ref uname, ref pass); // signup page where it takes user credentials

                    if (pass == "0") //if password and confirm password is not identical upon registration
                    {
                        Thread.Sleep(2000); pass = null;
                        goto Main;
                    }

                    while (unamechecker < accounts.Count) //checks is username is already taken by an account
                    {
                        if (accounts[unamechecker].Username == uname)
                        {
                            AlreadyExists();
                            goto Main;
                        }
                        else
                            unamechecker++;
                    }
                TypeSelection:
                    int type = TypeSelection(); // takes the number of the choice of the user regarding the type of account to be registered

                    switch (type)
                    {
                        case 1:
                            accounts.Add(new Student(name, uname, pass)); break;
                        case 2:
                            accounts.Add(new Worker(name, uname, pass)); break;
                        case 3:
                            accounts.Add(new House(name, uname, pass)); break;
                        case 4:
                            accounts.Add(new General(name, uname, pass)); break;
                        default:
                            PlayError(); TypeError(); // input does not correspond to the type choices in menu
                            goto TypeSelection;
                    }
                    SignupSuccessful();
                    goto Main;
                case 2:
                    string user = default, pword = default;
                    int k = default, i = 0, homeKey;
                    bool LoginAuth = default;

                    LoginMenu(ref user, ref pword); // login page

                    while (i < accounts.Count) // login authenticator
                    {
                        if (accounts[i].Username == user && accounts[i].Password == pword)
                        {
                            LoginSuccess();
                            LoginAuth = true;
                            break;
                        }
                        else
                        {
                            LoginAuth = false;
                            i++;
                        }
                    }

                    if (LoginAuth == false) // if login credentials doesn't match to any present account
                    {
                        LoginFailed();
                        goto Main;
                    }
                    else // if login credentials are okay
                    {
                        Thread.Sleep(2000);
                    Home:

                        Console.Clear();
                        Title();
                        accounts.ElementAt(i).Greet(); // greets user
                        DT(); //prints the current date and time 
                        accounts.ElementAt(i).RemindUser(); //reminds the user about the remaining tasks
                        homeKey = HomePage(); // homepage 

                        switch (homeKey) // options after homepage
                        {
                            case 7: // go back to main menu
                                goto Main;
                            case 1:
                                int ta = TaskAddMenu(); // add task

                                if (ta == 1) // import templated tasks for each account type
                                {
                                    bool present = accounts.ElementAt(i).ImportedPresent(); // bool to check if templated tasks are still present in the current list

                                    if (!present) // templated tasks are not present in the list
                                    {
                                        string Acctype = accounts.ElementAt(i).GetType().Name; // gets the type of account depending on the integer input
                                        PlaySuccess();
                                        accounts.ElementAt(i).ImportGeneralTask(); // imports general tasks

                                        switch (Acctype) // imports templated tasks specified by acount type
                                        {
                                            case "Student":
                                                accounts.ElementAt(i).ImportStudentTask(); break;
                                            case "Worker":
                                                accounts.ElementAt(i).ImportWorkTask(); break;
                                            case "House":
                                                accounts.ElementAt(i).ImportHouseTask(); break;
                                        }
                                        White("Press any key to go back to menu ", 27, 16);
                                        Console.ReadKey();
                                    }
                                    else
                                        ImportFailed();
                                }
                                else // add tasks manually inputted by user
                                {
                                    bool AddTask = accounts.ElementAt(i).AddTask();

                                    if (AddTask)
                                        TaskAddSuccess();
                                    else
                                    {
                                        White("Press any key to go back to menu ", 27, 19);
                                        Console.ReadKey();
                                    }
                                }
                                goto Home;
                            case 2: // task remover
                                int removed = 0;

                                Console.Clear();
                                Title();
                                accounts.ElementAt(i).ShowTask(ref k);
                                int taskremove = TaskRemove(ref k);
                                accounts.ElementAt(i).DeleteTask(taskremove, ref removed, ref k);
                                RemoveTaskSuccess(removed, ref k);
                                goto Home;
                            case 3: // displays To-Do List
                                Console.Clear();
                                Title();
                                accounts.ElementAt(i).ShowTask(ref k);
                                White("Press Any Key to Return to Menu ", 27, k + 1);
                                Console.ReadKey();
                                goto Home;
                            case 4: // search task
                                string search = default; int index = default; int pili = default;

                                TaskSearch(ref search, ref index, ref pili);

                                if (pili == 1) // search by name
                                    accounts.ElementAt(i).SearchTask(search);
                                else if (pili == 2) // search by index
                                    accounts.ElementAt(i).SearchTask(index);

                                Console.ReadKey();
                                goto Home;
                            case 6: // view profile
                                bool profile = accounts.ElementAt(i).Profile(accounts.ElementAt(i).GetType().Name);
                                if (profile)
                                {
                                    White("Press Any Key to Return to Menu ", 27, 22);
                                    Console.ReadKey();
                                    goto Home;
                                }
                                else
                                    goto Main;
                            case 5: // rearrange tasks
                                Console.Clear();
                                Title();
                                accounts.ElementAt(i).ShowTask(ref k);
                                accounts.ElementAt(i).SwapTask(ref k);
                                White("Press Any Key to Return to Menu ", 27, 29);
                                Console.ReadKey();
                                goto Home;
                            default:
                                PlayError(); goto Home;
                        }
                    }
                default:
                    goto Main;
            }
            Console.ReadKey();
        }

        // STATIC METHODS - UTILITY
        public static void LineClear(int y)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', Console.WindowWidth));
        }
        public static void PlaySuccess()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            string wavSuccess = System.IO.Path.Combine(strWorkPath, "success3.wav");
            SoundPlayer success = new SoundPlayer(wavSuccess);
            success.Play();
        }
        public static void PlayError()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            string wavError = System.IO.Path.Combine(strWorkPath, "invalid-selection-39351.wav");
            SoundPlayer error = new SoundPlayer(wavError);
            error.Play();
        }
        static void DT()
        {
            DateTime now = DateTime.Now;
            White("Today is ", 27, 11);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(now.ToString("f"));
            Console.ResetColor();
        }

        // STATIC METHODS - VISUALS
        public static void Red(string s, int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x, y); Console.Write(s);
            Console.ResetColor();
        }
        public static void Green(string s, int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(x, y); Console.Write(s);
            Console.ResetColor();
        }
        public static void Blue(string s, int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition(x, y); Console.Write(s);
            Console.ResetColor();
        }
        public static void Yellow(string s, int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(x, y); Console.Write(s);
            Console.ResetColor();
        }
        public static void White(string s, int x, int y)
        {
            Console.ResetColor();
            Console.SetCursorPosition(x, y); Console.Write(s);
        }
        public static void DarkRed(string s, int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(x, y); Console.Write(s);
            Console.ResetColor();
        }
        public static void Title()
        {
            Yellow(" ████████╗ ██████╗       ██████╗  ██████╗     ██╗     ██╗███████╗████████╗", 26, 1);
            Yellow(" ╚══██╔══╝██╔═══██╗      ██╔══██╗██╔═══██╗    ██║     ██║██╔════╝╚══██╔══╝", 26, 2);
            Yellow("    ██║   ██║   ██║█████╗██║  ██║██║   ██║    ██║     ██║███████╗   ██║   ", 26, 3);
            Yellow("    ██║   ██║   ██║╚════╝██║  ██║██║   ██║    ██║     ██║╚════██║   ██║   ", 26, 4);
            Yellow("    ██║   ╚██████╔╝      ██████╔╝╚██████╔╝    ███████╗██║███████║   ██║   ", 26, 5);
            Yellow("    ╚═╝    ╚═════╝       ╚═════╝  ╚═════╝     ╚══════╝╚═╝╚══════╝   ╚═╝   ", 26, 6);
            Red("+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+", 27, 7);
        }
        static void Signup()
        {
            Yellow("███████╗██╗ ██████╗ ███╗   ██╗      ██╗   ██╗██████╗ ", 37, 1);
            Yellow("██╔════╝██║██╔════╝ ████╗  ██║      ██║   ██║██╔══██╗", 37, 2);
            Yellow("███████╗██║██║  ███╗██╔██╗ ██║█████╗██║   ██║██████╔╝", 37, 3);
            Yellow("╚════██║██║██║   ██║██║╚██╗██║╚════╝██║   ██║██╔═══╝ ", 37, 4);
            Yellow("███████║██║╚██████╔╝██║ ╚████║      ╚██████╔╝██║  ", 37, 5);
            Yellow("╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═══╝       ╚═════╝ ╚═╝ ", 37, 6);
            Red("+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+", 36, 7);
        }
        static void Login()
        {
            Yellow("██╗      ██████╗  ██████╗       ██╗███╗   ██╗", 41, 1);
            Yellow("██║     ██╔═══██╗██╔════╝       ██║████╗  ██║", 41, 2);
            Yellow("██║     ██║   ██║██║  ███╗█████╗██║██╔██╗ ██║", 41, 3);
            Yellow("██║     ██║   ██║██║   ██║╚════╝██║██║╚██╗██║", 41, 4);
            Yellow("███████╗╚██████╔╝╚██████╔╝      ██║██║ ╚████║", 41, 5);
            Yellow("╚══════╝ ╚═════╝  ╚═════╝       ╚═╝╚═╝  ╚═══╝", 41, 6);
            Red("+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x+x", 41, 7);
        }
        static void EndScreen()
        {
            Yellow("████████╗██╗  ██╗ █████╗ ███╗   ██╗██╗  ██╗    ██╗   ██╗ ██████╗ ██╗   ██╗██╗", 25, 10);
            Yellow("╚══██╔══╝██║  ██║██╔══██╗████╗  ██║██║ ██╔╝    ╚██╗ ██╔╝██╔═══██╗██║   ██║██║", 25, 11);
            Yellow("   ██║   ███████║███████║██╔██╗ ██║█████╔╝      ╚████╔╝ ██║   ██║██║   ██║██║", 25, 12);
            Yellow("   ██║   ██╔══██║██╔══██║██║╚██╗██║██╔═██╗       ╚██╔╝  ██║   ██║██║   ██║╚═╝", 25, 13);
            Yellow("   ██║   ██║  ██║██║  ██║██║ ╚████║██║  ██╗       ██║   ╚██████╔╝╚██████╔╝██╗", 25, 14);
            Yellow("   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝       ╚═╝    ╚═════╝  ╚═════╝ ╚═╝", 25, 15);
        }

        // STATIC METHODS - MENU
        static void SignupMenu(ref string name, ref string uname, ref string pass)
        {
            ConsoleKeyInfo key1, key2;
            List<char> passuno = new List<char>();
            List<char> passdos = new List<char>();
            int i = 0, j = 0;

            Console.Clear();
            Signup();

            White("Enter Name: ", 36, 9);
            name = Console.ReadLine();
            White("Enter Username: ", 36, 10);
            uname = Console.ReadLine();
            White("Enter password: ", 36, 11);
            
            do
            {
                key1 = Console.ReadKey(true);

                if (key1.Key != ConsoleKey.Backspace)
                {
                    passuno.Add(key1.KeyChar);
                    Console.Write("*");
                    i++;
                }
                else
                {
                    try
                    {
                        Console.Write("\b \b");
                        passuno.RemoveAt(i - 1);
                        i--;
                    }
                    catch
                    {
                        PlayError();
                        Console.Write(" ");
                    }
                }
            } while (key1.Key != ConsoleKey.Enter);

            string pass1 = string.Concat(passuno);

            White("Confirm password: ", 36, 12);
            do
            {
                key2 = Console.ReadKey(true);

                if (key2.Key != ConsoleKey.Backspace)
                {
                    passdos.Add(key2.KeyChar);
                    Console.Write("*");
                    j++;
                }
                else
                {
                    try
                    {
                        Console.Write("\b \b");
                        passdos.RemoveAt(j - 1);
                        j--;
                    }
                    catch
                    {
                        PlayError();
                        Console.Write(" ");
                    }
                }
            } while (key2.Key != ConsoleKey.Enter);

            string pass2 = string.Concat(passdos);

            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(uname) || String.IsNullOrWhiteSpace(pass1) || String.IsNullOrWhiteSpace(pass2))
            {
                PlayError();
                Red("Blank Space Inputs are not allowed. Please Try Again.", 36, 14);
                pass = "0";
                name = null;
                uname = null;
                return;
            }

            if (pass1 == pass2)
            {
                pass = pass1;
            }
            else
            {
                PlayError();
                Red("Passwords Do Not Match. Please Try Again.", 36, 14);
                pass = "0"; name = null; uname = null;
            }

        }
        static int TypeSelection()
        {
        TypeSelection:
            try
            {
                White("Are you a", 36, 14);
                White("(1) Student       (3) Housekeeper ", 36, 16);
                White("(2) Worker        (4) General Use", 36, 17);
                White("→ ", 36, 19);
                return int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                PlayError();
                Red($"{e.Message} Please Try Again.", 36, 20);
                Thread.Sleep(1000);
                LineClear(20); LineClear(19);
                goto TypeSelection;
            }
        }
        static int TitleMenu()
        {
        LoginMenu:
            Console.Clear();
            Title();
            try
            {
                White("(0) Exit", 57, 10);
                White("(1) Signup", 57, 11);
                White("(2) Login", 57, 12);
                White("→ ", 57, 14);
                return int.Parse(Console.ReadLine());
            }
            catch
            {
                PlayError();
                goto LoginMenu;
            }
        }
        static void LoginMenu(ref string user, ref string pword)
        {
        LoginMenu:
            try
            {
                List<char> pd = new List<char>();
                ConsoleKeyInfo key;
                int i = 0;

                Console.Clear();
                Login();
                White("Enter Username: ", 41, 9);
                user = Console.ReadLine();
                White("Enter Password: ", 41, 10);
                
                do
                {
                    key = Console.ReadKey(true);

                    if (key.Key != ConsoleKey.Backspace)
                    {
                        pd.Add(key.KeyChar);
                        Console.Write("*");
                        i++;
                    }
                    else
                    {
                        try
                        {
                            Console.Write("\b \b");
                            pd.RemoveAt(i - 1);
                            i--;
                        }
                        catch
                        {
                            PlayError();
                            Console.Write(" ");
                        } 
                    }
                } while (key.Key != ConsoleKey.Enter);

                pword = string.Concat(pd);
            }
            catch
            {
                PlayError();
                goto LoginMenu;
            }
        }
        static int HomePage()
        {
            White("------------------------------<<(Menu)>>---------------------------------", 27, 15);
            White("(1) Add a Task                                   (5) Rearrange Task    ", 27, 17);
            White("(2) Delete a Task                                (6) Profile ", 27, 18);
            White("(3) View Sorted Tasks                            (7) Log-out ", 27, 19);
            White("(4) Search Task in List", 27, 20);
            White("-------------------------------------------------------------------------", 27, 22);
            White("→ ", 27, 23);

            try
            {
                return int.Parse(Console.ReadLine());
            }
            catch
            {
                PlayError();
                return 0;
            }
        }
        static int TaskAddMenu()
        {
            Console.Clear();
            Title();
            White("----------------------------<<(ADD TASK)>>-------------------------------", 27, 9);
            return TaskAdd();
        }
        static int TaskAdd()
        {
        TaskAdd:
            try
            {
                White("Import Templated Tasks or Add Tasks Manually? (1/2) → ", 27, 11);
                int AddT = int.Parse(Console.ReadLine());

                switch (AddT)
                {
                    case 1:
                        return AddT;
                    case 2:
                        return AddT;
                    default:
                        PlayError();
                        Red("Invalid Input, Please Try Again", 27, 13);
                        Thread.Sleep(1000);
                        LineClear(11); LineClear(13);
                        goto TaskAdd;
                }
            }
            catch (Exception e)
            {
                PlayError();
                Red($"{e.Message} Please Try Again.", 27, 13);
                Thread.Sleep(1000);
                LineClear(11); LineClear(13);
                goto TaskAdd;
            }
        }
        static int TaskRemove(ref int k)
        {
        TaskRemove:
            try
            {
                White("Which Task Would You Like to Remove? → ", 27, k + 1);
                return int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                PlayError();
                Red($"{e.Message} Please Try Again.", 27, k + 3);
                Thread.Sleep(1000);
                LineClear(k + 3); LineClear(k + 1);
                goto TaskRemove;
            }
        }
        static void TaskSearch(ref string search, ref int index, ref int n)
        {
        TaskSearch:
            Console.Clear();
            Title();
            White("----------------------------<<(Search Task)>>----------------------------", 27, 10);
            White("Do you want to search by (1) Name or (2) Index → ", 27, 12);

            try
            {
                n = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                PlayError();
                Red($"{e.Message} Please Try Again.", 27, 14);
                Thread.Sleep(1000);
                goto TaskSearch;
            }
            if (n == 1)
            {
                White("Enter Task Name To Search: ", 27, 15);
                search = Console.ReadLine();
            }
            else if (n == 2)
            {
            TaskNo:
                White("Enter Task Number: ", 27, 15);
                try
                {
                    index = int.Parse(Console.ReadLine());
                }
                catch (Exception e)
                {
                    PlayError();
                    Red($"{e.Message} Please Try Again", 27, 17);
                    Thread.Sleep(1000);
                    LineClear(17); LineClear(15);
                    goto TaskNo;
                }
            }
            else
            {
                PlayError();
                Red("Wrong Input. Input 1 or 2 only", 27, 14);
                Thread.Sleep(1000);
                goto TaskSearch;
            }

        }

        // STATIC METHODS - MESSAGES
        static void AlreadyExists()
        {
            PlayError();
            Red("Username already exists!", 36, 14);
            Red("Try logging in or registering with another username.", 36, 15);
            Thread.Sleep(2000);
        }
        static void SignupSuccessful()
        {
            PlaySuccess();
            Green("✓ Signup Successful!", 36, 21);
            White("Press Any Key To Go Back To Main Menu ", 36, 22);
            Console.ReadKey();
        }
        static void TypeError()
        {
            Red("No Account Type Exists with your input. Please Try Again.", 36, 20);
            Thread.Sleep(1000);
            LineClear(20); LineClear(19);
        }
        static void LoginSuccess()
        {
            PlaySuccess();
            Green("✓ Login Successful! Redirecting to Home Page", 41, 12);
        }
        static void LoginFailed()
        {
            PlayError();
            Red("Login unsuccessful.", 41, 12);
            Red("Unexisting username or incorrect password.", 41, 13);
            White("Please enter any key to try again ", 41, 15);
            Console.ReadKey();
        }
        static void TaskAddSuccess()
        {
            PlaySuccess();
            Green("✓ Task Added! ", 27, 17);
            White("Press Any Key to Return to Menu", 27, 19);
            Console.ReadKey();
        }
        static void ImportFailed()
        {
            PlayError();
            Red("Cannot import templated tasks.", 27, 14);
            Red("You still have templated tasks present in your list.", 27, 15);
            White("Press any key to go back to Menu", 27, 16);
            Console.ReadKey();
        }
        static void RemoveTaskSuccess(int removed, ref int k)
        {
            if (removed != 0)
            {
                PlaySuccess();
                Green("✓ Task Removed!", 27, k + 3);
            }
            White("Press Any Key to Return to Menu ", 27, k + 4);
            Console.ReadKey();
        }
    }

    // INTERFACES
    interface IStudent
    {
        void ImportStudentTask();
    }
    interface IWork
    {
        void ImportWorkTask();
    }
    interface IHouse
    {
        void ImportHouseTask();
    }
    interface IGeneral
    {
        void ImportGeneralTask();
    }

    // CLASSES
    abstract class Account : IGeneral, IHouse, IWork, IStudent
    {
        private string name, username, password;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public Account(string name, string username, string password)
        {
            Name = name;
            Username = username;
            Password = password;
        }

        // ABSTRACT METHODS
        public abstract void Greet();
        public abstract bool AddTask();
        public abstract void ShowTask(ref int k);
        public abstract void DeleteTask(int removetask, ref int removed, ref int k);
        public abstract void RemindUser();
        public abstract void ImportGeneralTask();
        public abstract void SearchTask(string search);
        public abstract void SearchTask(int index);
        public abstract bool ImportedPresent();
        public abstract bool Profile(string type);
        public abstract void SwapTask(ref int k);

        // VIRTUAL METHODS
        public virtual void ImportStudentTask() { }
        public virtual void ImportWorkTask() { }
        public virtual void ImportHouseTask() { }

    }
    class General : Account
    {
        protected List<String> important = new List<String>();
        protected List<String> unimportant = new List<String>();
        protected string[] GeneralTask = new string[4] { "Eat Your Meals", "Brush Your Teeth", "Take a Bath", "Take a Break" };
        protected string[] StudentTask = new string[4] { "Attend Classes", "Study for upcoming quizzes/exams", "Do Homework", "Advance Study" };
        protected string[] WorkerTask = new string[4] { "Attend Meetings", "Finish Assigned Tasks", "Check on Your Team", "Consult Manager" };
        protected string[] HouseTask = new string[4] { "Clean The House", "Cook Food For Meals", "Buy Groceries", "Check Appliances and Furnitures" };
        public General(string name, string username, string password) : base(name, username, password) { }

        public override void Greet()
        {
            Program.White("Hello User ", 27, 9);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{Name}");
            Console.ResetColor();
            Console.WriteLine("! Welcome to your To-Do-List!");
        }
        public override bool ImportedPresent()
        {
            bool present = default;
            for (int i = 0; i < 4; i++)
            {
                if (important.Contains(GeneralTask[i]) || unimportant.Contains(GeneralTask[i]))
                    present = true;
                else if (important.Contains(StudentTask[i]) || unimportant.Contains(StudentTask[i]))
                    present = true;
                else if (important.Contains(WorkerTask[i]) || unimportant.Contains(WorkerTask[i]))
                    present = true;
                else if (important.Contains(HouseTask[i]) || unimportant.Contains(HouseTask[i]))
                    present = true;
                else
                    present = false;
            }
            return present;
        }
        public override bool AddTask()
        {
            int mi;
        AddTask:
            try
            {
                Program.White("Is the task ", 27, 13);
                Console.ForegroundColor = ConsoleColor.Blue; Console.Write("(1) Unimportant");
                Console.ResetColor(); Console.Write(" or ");
                Console.ForegroundColor = ConsoleColor.Red; Console.Write("(2) Important? ");
                Console.ResetColor(); Console.Write("→ ");
                mi = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Program.PlayError();
                Program.Red($"{e.Message} Please Try Again.", 27, 15);
                Thread.Sleep(1000);
                Program.LineClear(13); Program.LineClear(15);
                goto AddTask;
            }

            switch (mi)
            {
                case 1:
                    Program.White("Enter new task name: ", 27, 15);
                    string task = Console.ReadLine();
                    int y = unimportant.IndexOf(task); int x = important.IndexOf(task);

                    if (y != -1 || x != -1)
                    {
                        Program.PlayError();
                        Program.Red("Task Already Exists! Please Search Task or View Task in Your List. ", 27, 17);
                        return false;
                    }
                    else if (String.IsNullOrWhiteSpace(task))
                    {
                        Program.PlayError();
                        Program.Red("Invalid Input, Please Try Again.", 27, 17);
                        Thread.Sleep(1000);
                        Program.LineClear(15); Program.LineClear(13); Program.LineClear(17);
                        goto AddTask;
                    }
                    else
                    {
                        unimportant.Add(task);
                        return true;
                    }
                case 2:
                    Program.White("Enter new task name: ", 27, 15);
                    string buhat = Console.ReadLine();
                    int e = important.IndexOf(buhat); int r = unimportant.IndexOf(buhat);

                    if (e != -1 || r != -1)
                    {
                        Program.PlayError(); Console.ForegroundColor = ConsoleColor.Red;
                        Console.SetCursorPosition(27, 17); Console.Write("Task Already Exists! Please Search Task or View Task in Your List. ");
                        Console.ResetColor(); return false;
                    }
                    else if (String.IsNullOrWhiteSpace(buhat))
                    {
                        Program.PlayError();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.SetCursorPosition(27, 17); Console.Write("Invalid Input, Please Try Again.");
                        Thread.Sleep(1000);
                        Program.LineClear(15); Program.LineClear(13); Program.LineClear(17);
                        Console.ResetColor();
                        goto AddTask;
                    }
                    else
                    {
                        important.Add(buhat);
                        return true;
                    }
                default:
                    Program.PlayError();
                    Program.Red("Input 1 or 2 only!", 27, 15);
                    Thread.Sleep(1000);
                    Program.LineClear(15); Program.LineClear(13);
                    goto AddTask;
            }
        }
        public override void ShowTask(ref int k)
        {
            int i = 0, j = 0; k = 15;

            Program.White("Sorted Tasks from Time Added and Importance:", 42, 10);
            Program.Red("Red = Important", 27, 12);
            Program.Blue("Blue = Unimportant", 45, 12);
            Program.White("-----------------------------<<(TASKS)>>---------------------------------", 27, 14);

            while (i < important.Count)
            {
                Program.Red($"({i + 1})", 27, k);
                Console.WriteLine($" {important.ElementAt(i)}");
                i++; k++;
            }
            while (j < unimportant.Count)
            {
                Program.Blue($"({i + 1})", 27, k);
                Console.WriteLine($" {unimportant.ElementAt(j)}");
                j++; i++; k++;
            }
            Program.White("-------------------------------------------------------------------------", 27, k);
        }
        public override void DeleteTask(int removetask, ref int removed, ref int k)
        {
            try
            {
                if (removetask <= important.Count)
                {
                    important.RemoveAt(removetask - 1);
                    removed = 1;
                }
                else if (removetask > important.Count && removetask - important.Count <= unimportant.Count)
                {
                    unimportant.RemoveAt(removetask - important.Count - 1);
                    removed = 1;
                }
                else
                {
                    Program.PlayError();
                    removed = 0;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Program.Red("Cannot delete task because task does not exist.", 27, k + 3);
                }
            }
            catch
            {
                Program.PlayError();
                Program.Red("Task Number Starts With 1", 27, k + 3);
                Thread.Sleep(1000);
            }

        }
        public override void ImportGeneralTask()
        {
            for (int i = 0; i < GeneralTask.Length; i++)
            {
                if (i != 3)
                    important.Add(GeneralTask[i]);
                else
                    unimportant.Add(GeneralTask[i]);
            }

            Program.Green("Common Tasks have been imported to your To-Do List!", 27, 13);
        }
        public override void RemindUser()
        {
            Program.White($"You have ", 27, 13);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{important.Count + unimportant.Count}"); Console.ResetColor();
            Console.Write(" tasks to finish.");
        }
        public override void SearchTask(string search)
        {
            var targetList = important.Concat(unimportant).ToList();
            int y = 17, counter = 0;

            for (int i = 0; i < targetList.Count; i++)
            {
                if (Regex.IsMatch(targetList.ElementAt(i), search, RegexOptions.IgnoreCase))
                {
                    if (i < important.Count)
                    {
                        Program.Red($"({i + 1}) ", 27, ++y);
                        Console.WriteLine($"{important.ElementAt(i)}");
                        counter++;
                    }
                    else
                    {
                        Program.Blue($"({i + 1}) ", 27, ++y);
                        Console.WriteLine($"{unimportant.ElementAt(i - (important.Count))}");
                        counter++;
                    }
                }
            }
            if (counter > 0)
            {
                Program.PlaySuccess();
                Program.Green("✓ Tasks/s Found!", 27, 17);
                Program.White("Press Any Key to Return to Menu ", 27, y + 2);
            }
            else
            {
                Program.PlayError();
                Program.Red("Nothing Found!", 27, 17);
                Program.White("Press Any Key to Return to Menu ", 27, y + 2);
            }
        }
        public override void SearchTask(int index)
        {
            if (index <= important.Count + unimportant.Count)
            {
                if (index <= important.Count)
                {
                    Program.PlaySuccess();
                    Program.Green("✓ Task Found!", 27, 17);
                    Program.Red($"({index}) ", 27, 18);
                    Console.WriteLine(important.ElementAt(index - 1));
                    Program.White("Press Any Key to Return to Menu", 27, 20);
                }
                else if (index - important.Count <= unimportant.Count)
                {
                    Program.PlaySuccess();
                    Program.Green("✓ Task Found!", 27, 17);
                    Program.Blue($"({index}) ", 27, 18);
                    Console.WriteLine(unimportant.ElementAt(index - important.Count - 1));
                    Program.White("Press Any Key to Return to Menu", 27, 20);
                }
            }
            else
            {
                Program.PlayError();
                Program.Red("Nothing Found!", 27, 17);
                Program.White("Press Any Key to Return to Menu", 27, 18);
            }
        }
        public override void SwapTask(ref int k)
        {
            int a, b;
            var targetList = important.Concat(unimportant).ToList();

            try
            {
                Program.White("Enter first task number to swap: ", 27, k + 1);
                a = int.Parse(Console.ReadLine());
                Program.White("Enter second task number to swap: ", 27, k + 2);
                b = int.Parse(Console.ReadLine());

                (targetList[a - 1], targetList[b - 1]) = (targetList[b - 1], targetList[a - 1]);
            }
            catch
            {
                Program.PlayError();
                Program.Red("Invalid Input. Please Try Again", 27, k + 4);
                return;
            }

            for (int i = 0; i < important.Count; i++)
            {
                important[i] = targetList[i];
            }
            for (int j = 0; j < unimportant.Count; j++)
            {
                unimportant[j] = targetList[important.Count + j];
            }
            Program.PlaySuccess();
            Program.Green("✓ Swap Success! Tasks Rearranged.", 27, k + 4);
        }
        public override bool Profile(string type)
        {
        Profile:
            int Pchoice;
            Console.Clear();
            Program.Title();
            Program.White("---------------------------<<(USER PROFILE)>>----------------------------", 27, 10);
            Program.White($"Name: {Name}", 27, 11);
            Program.White("|         (1) Change Name", 63, 11);
            Program.White($"Account Type: {type}", 27, 12);
            Program.White("|         (2) Change Username", 63, 12);
            Program.White($"Username: {Username}", 27, 13);
            Program.White("|         (3) Change Password", 63, 13);
            Program.White($"Password: {Password}", 27, 14);
            Program.White("|         (4) Delete Account", 63, 14);
            Program.White("|         (5) Back To Menu", 63, 15);
            Program.White($"Tasks Remaining: {important.Count + unimportant.Count}", 27, 15);
            Program.White("-------------------------------------------------------------------------", 27, 16);
            Program.White("→ ", 27, 17);
            try
            {
                Pchoice = int.Parse(Console.ReadLine());
            }
            catch
            {
                Program.PlayError();
                goto Profile;
            }

            switch (Pchoice)
            {
                case 1:
                    Program.White("Enter New Name: ", 27, 19);
                    string name = Console.ReadLine();
                    if (!String.IsNullOrWhiteSpace(name))
                    {
                        Name = name;
                        Program.PlaySuccess();
                        Program.Green("✓ Name Updated!", 27, 21);
                    }
                    else
                    {
                        Program.PlayError();
                        Program.Red("Invalid Input!", 27, 21);
                    }
                    return true;
                case 2:
                    Program.White("Enter New Username: ", 27, 19);
                    string username = Console.ReadLine();
                    if (!String.IsNullOrWhiteSpace(username))
                    {
                        Username = username;
                        Program.PlaySuccess();
                        Program.Green("✓ Username Updated!", 27, 21);
                    }
                    else
                    {
                        Program.PlayError();
                        Program.Red("Invalid Input!", 27, 21);
                    }
                    return true;
                case 3:
                    Program.White("Enter New Password: ", 27, 19);
                    ConsoleKeyInfo key;
                    List<char> pass = new List<char>();
                    int i = 0;

                    do
                    {
                        key = Console.ReadKey(true);

                        if (key.Key != ConsoleKey.Backspace)
                        {
                            pass.Add(key.KeyChar);
                            Console.Write("*");
                            i++;
                        }
                        else
                        {
                            try
                            {
                                Console.Write("\b \b");
                                pass.RemoveAt(i - 1);
                                i--;
                            }
                            catch
                            {
                                Program.PlayError();
                                Console.Write(" ");
                            }
                        }
                    } while (key.Key != ConsoleKey.Enter);

                    string password = string.Concat(pass);

                    if (!String.IsNullOrWhiteSpace(password))
                    {
                        Password = password;
                        Program.PlaySuccess();
                        Program.Green("✓ Password Updated!", 27, 21);
                    }
                    else
                    {
                        Program.PlayError();
                        Program.Red("Invalid Input!", 27, 21);
                    }
                    return true;
                case 4:
                    Name = null; Username = null; Password = null;
                    Program.PlaySuccess();
                    Program.Green("✓ Account Deleted!", 27, 19);
                    Program.White("Redirecting to Home Page.", 27, 20);
                    Thread.Sleep(2000);
                    return false;
                case 5:
                    return true;
                default: Program.PlayError(); goto Profile;
            }
        }

    }
    class Student : General, IStudent
    {
        public Student(string name, string username, string password) : base(name, username, password) { }

        public override void Greet()
        {
            Program.White("Hello Student ", 27, 9);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{Name}");
            Console.ResetColor();
            Console.WriteLine("! Welcome to your To-Do-List!");
        }
        public override void ImportStudentTask()
        {
            for (int i = 0; i < StudentTask.Length; i++)
            {
                if (i != 3)
                    important.Add(StudentTask[i]);
                else
                    unimportant.Add(StudentTask[i]);
            }
            Program.Green("Common Student Tasks have been imported to your To-Do List!", 27, 14);
        }
    }
    class Worker : General, IWork
    {
        public Worker(string name, string username, string password) : base(name, username, password) { }

        public override void Greet()
        {
            Program.White("Hello Worker ", 27, 9);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{Name}");
            Console.ResetColor();
            Console.WriteLine("! Welcome to your To-Do-List!");
        }
        public override void ImportWorkTask()
        {
            for (int i = 0; i < WorkerTask.Length; i++)
            {
                if (i != 3)
                    important.Add(WorkerTask[i]);
                else
                    unimportant.Add(WorkerTask[i]);
            }
            Program.Green("Common Worker Tasks have been imported to your To-Do List!", 27, 14);
        }
    }
    class House : General, IHouse
    {
        public House(string name, string username, string password) : base(name, username, password) { }

        public override void Greet()
        {
            Program.White("Hello Housekeeper ", 27, 9);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{Name}");
            Console.ResetColor();
            Console.WriteLine("! Welcome to your To-Do-List!");
        }
        public override void ImportHouseTask()
        {
            for (int i = 0; i < HouseTask.Length; i++)
            {
                important.Add(HouseTask[i]);
            }
            Program.Green("Common House Tasks have been imported to your To-Do List!", 27, 14);
        }
    }
}
