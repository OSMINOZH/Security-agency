using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Diplom
{
    public partial class MainMenu : MaterialForm
    {
        public bool carPanelOpen = false, ndvPanelOpen = false, clientPanelOpen = false, gbrPanelOpen = false, contractPanelOpen = false, alarmPanelOpen = false; // Все панели
        public int SensorTriggered = 0, timerTo = 900, timerToMchs = 900, timerToRand = 150, randTime = 0; // Время до ГБР (15 минут) // Все счётчики
        public bool DisableIgnition = false, DisableSensors = false, EmergencyResponseTeam = false, DataFromDB = false, MchsSent = false, RandSelEnabled = false; // Элементы управления
        public Random random = new Random(); // Рандомизирует переменную randTime
        //CAR BELOW
        public string carID, carModel, carBrand, carNumbers; // Empty for the start
        //CAR ENDS HERE
        //NDV BELOW
        public string ndvID, ndvCity, ndvStreet, ndvHouse, ndvAppart; // Empty for the start
        //NDV ENS HERE

        public MainMenu(string username)
        {
            InitializeComponent();
            ShowHidePanels("All");
            this.security.EndInit();
            UserInfoLABEL.Text = username;
            // MaterialSkin // Ниже код визуального интерфейса
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }
        private void MainMenu_Load(object sender, EventArgs e) // Загружает данные из бд в таблицу
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.Car". При необходимости она может быть перемещена или удалена.
            this.carTableAdapter.Fill(this.security.Car);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.ObjectTable". При необходимости она может быть перемещена или удалена.
            this.objectTableTableAdapter.Fill(this.security.ObjectTable);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.Triggering". При необходимости она может быть перемещена или удалена.
            this.triggeringTableAdapter.Fill(this.security.Triggering);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.Triggering". При необходимости она может быть перемещена или удалена.
            this.triggeringTableAdapter.Fill(this.security.Triggering);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.Contract". При необходимости она может быть перемещена или удалена.
            this.contractTableAdapter.Fill(this.security.Contract);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.Staff". При необходимости она может быть перемещена или удалена.
            this.staffTableAdapter.Fill(this.security.Staff);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.RrtStaff". При необходимости она может быть перемещена или удалена.
            this.rrtStaffTableAdapter.Fill(this.security.RrtStaff);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.RRT". При необходимости она может быть перемещена или удалена.
            this.rRTTableAdapter.Fill(this.security.RRT);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "security.Client". При необходимости она может быть перемещена или удалена.
            this.clientTableAdapter.Fill(this.security.Client);

        }

        private void MainMenu_FormClosed(object sender, FormClosedEventArgs e) // Standart closing form // Закрывает форму и чистит логи
        {
            File.WriteAllText(@"log.txt", string.Empty); // Cleares log file // Delete after tests // !!!!!!!!!!!!!!!!!!!!!!!!!!
            this.Close();
        } 

        private void timer1_Tick(object sender, EventArgs e) // Таймер // Раз в секунду выполняет какой-то код
        {
            if (RandSelEnabled && timerToRand!=0) // Рандомизация времени запуска датчиков // ТЕСТ
            {
                randTime--;

                if (randTime <= 0)
                {
                    RandomiseSelection(); // Вызывает раз в сколько-то секунд
                    randTime = random.Next(1, 16); // Генерация случайного числа от 1 до 15 секунд
                }
            }
            if ((SensorTriggered >= 3) && (!EmergencyResponseTeam)) // Не дает вызывать ГБР если больше 3-х срабатываний или если гбр уже едет
            {
                if (carPanelOpen)
                {
                    CreateLog("MultipleSensors", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), "");
                    SentEmergencyBTN_Click(sender, e);
                }
                else if (ndvPanelOpen)
                {
                    CreateLog("NdvMultipleSensors", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), "");
                    EmergencyTeamSentNdvBTN_Click(sender, e);
                }
            }
            if ((EmergencyResponseTeam) && (timerTo != 0)) // ГБР
            {
                timerTo -= 1; // Отсчёт прибытия ГБР
                if (carPanelOpen)
                {
                    CurrTimeToGbrLABEL.Text = (timerTo / 60).ToString();
                }
                else if (ndvPanelOpen)
                {
                    NdvCurrTimeToGbrLABEL.Text = (timerTo / 60).ToString();
                }
            }
            else if ((EmergencyResponseTeam) && (timerTo == 0)) // Когда уже ГБР прибыла
            {
                if (carPanelOpen)
                {
                    CreateLog("GbrArrived", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
                    CurrStatusLABEL.Text = "ГБР прибыла на место происшествия";
                }
                else if (ndvPanelOpen)
                {
                    CreateLog("GbrArrived", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
                    NdvCurrStatusLABEL.Text = "ГБР прибыла на место происшествия";
                }
                EmergencyResponseTeam = false;
                timerTo = 900; // Сброс значения
            }
            if ((MchsSent) && (timerToMchs != 0)) // МЧС
            {
                timerToMchs -= 1; // Отсчёт прибытия МЧС
            }
            else if ((MchsSent) && (timerToMchs == 0))
            {
                CreateLog("MchsArrived", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
                NdvCurrStatusLABEL.Text = "МЧС прибыл на охраняемый объект";
                MchsSent = false;
                timerToMchs = 900; // Сброс значения
            }
            CarLogTB.Text = File.ReadAllText(@"log.txt").ToString(); // Refreshes log.txt
            NdvLogTB.Text = File.ReadAllText(@"log.txt").ToString(); // Refreshes log.txt
        }

        // Randomise Selection Below // Связано с ТЕСТОМ (рандомизация)
        private void LockAllRadBTN(bool enable) // Скрывает кнопки на панелях при тесте
        {
            if (enable)
            {
                if (carPanelOpen)
                {
                    carRandomiseBTN.Visible = false;
                    endCarRandomiseBTN.Visible = true;
                    FrontDoorSensorRBTN.Enabled = false;
                    RearDoorSensorRBTN.Enabled = false;
                    HoodSensorRBTN.Enabled = false;
                    TrunkSensorRBTN.Enabled = false;
                    BatterySensorRBTN.Enabled = false;
                    IgnitionRBTN.Enabled = false;
                }
                else if (ndvPanelOpen)
                {
                    ndvRandomiseBTN.Visible = false;
                    endNdvRandomiseBTN.Visible = true;
                    DipRBTN.Enabled = false;
                    IkRBTN.Enabled = false;
                    KtsRBTN.Enabled = false;
                    CmkRBTN.Enabled = false;
                    DygRBTN.Enabled = false;
                    DrsRBNT.Enabled = false;
                    DyvRBTN.Enabled = false;
                }
            }
            else if (!enable)
            {
                if (carPanelOpen)
                {
                    carRandomiseBTN.Visible = true;
                    endCarRandomiseBTN.Visible = false;
                    FrontDoorSensorRBTN.Enabled = true;
                    RearDoorSensorRBTN.Enabled = true;
                    HoodSensorRBTN.Enabled = true;
                    TrunkSensorRBTN.Enabled = true;
                    BatterySensorRBTN.Enabled = true;
                    IgnitionRBTN.Enabled = true;
                }
                else if (ndvPanelOpen)
                {
                    ndvRandomiseBTN.Visible = true;
                    endNdvRandomiseBTN.Visible = false;
                    DipRBTN.Enabled = true;
                    IkRBTN.Enabled = true;
                    KtsRBTN.Enabled = true;
                    CmkRBTN.Enabled = true;
                    DygRBTN.Enabled = true;
                    DrsRBNT.Enabled = true;
                    DyvRBTN.Enabled = true;
                }
            }
        }
        private void RandomiseSelection() // Рандомизация
        {
            if (carPanelOpen)
            {
                int randomNumber = random.Next(1, 7);
                switch (randomNumber)
                {
                    case 1:
                        FrontDoorSensorRBTN_Click(FrontDoorSensorRBTN, null);
                        break;
                    case 2:
                        RearDoorSensorRBTN_Click(RearDoorSensorRBTN, null);
                        break;
                    case 3:
                        HoodSensorRBTN_Click(HoodSensorRBTN, null);
                        break;
                    case 4:
                        TrunkSensorRBTN_Click(TrunkSensorRBTN, null);
                        break;
                    case 5:
                        BatterySensorRBTN_Click(BatterySensorRBTN, null);
                        break;
                    case 6:
                        IgnitionRBTN_Click(IgnitionRBTN, null);
                        break;
                }
            }
            else if (ndvPanelOpen)
            {
                int randomNumber = random.Next(1, 8);
                switch (randomNumber)
                {
                    case 1:
                        DipRBTN_Click(DipRBTN, null);
                        break;
                    case 2:
                        IkRBTN_Click(IkRBTN, null);
                        break;
                    case 3:
                        KtsRBTN_Click(KtsRBTN, null);
                        break;
                    case 4:
                        CmkRBTN_Click(CmkRBTN, null);
                        break;
                    case 5:
                        DygRBTN_Click(DygRBTN, null);
                        break;
                    case 6:
                        DrsRBNT_Click(DrsRBNT, null);
                        break;
                    case 7:
                        DyvRBTN_Click(DyvRBTN, null);
                        break;
                }
            }
        }

        // Randomise Selection Ends Here

        // DataBase Control Below
        private string LoadData(string sql) // Запрос данных
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-LIU1R6H; Initial Catalog = Security; " + "Integrated Security=True;");
            con.Open();
            string sqlcom = sql;
            SqlCommand cmd = new SqlCommand(sqlcom, con);
            object data = cmd.ExecuteScalar();
            if (data != null)
            {
                string udata = data.ToString();
                con.Close();
                return udata;
            }
            else
            {
                con.Close();
                MessageBox.Show("Информация не найдена в базе данных");
            }
            return "null";
        }
        private void DBCC(string table, string idTable) // ONly client ID!!!!!
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-LIU1R6H; Initial Catalog = Security; " + "Integrated Security=True;");
            con.Open();
            string sqlcom = "declare @max int select @max=max([" + idTable + "])from [" + table + "] if @max IS NULL SET @max = 0 DBCC CHECKIDENT('[" + table + "]', RESEED,@max)";
            SqlCommand cmd = new SqlCommand(sqlcom, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        private void OK_Click(object sender, EventArgs e) // Если пустой ID то обнуляет информацию о машине и записи
        {
            if (!string.IsNullOrWhiteSpace(CarIdTB.Text))
            {
                DataFromDB = true;
                carModel = LoadData("SELECT carModel FROM Car WHERE CarID='" + CarIdTB.Text + "'").Replace(" ", "");
                carBrand = LoadData("SELECT carBrand FROM Car WHERE CarID='" + CarIdTB.Text + "'").Replace(" ", "");
                carNumbers = LoadData("SELECT carNumbers FROM Car WHERE CarID='" + CarIdTB.Text + "'").Replace(" ", "");
                carID = CarIdTB.Text;
                CarModelTB.Text = carModel;
                CarBrandTB.Text = carBrand;
                CarLicensePlateTB.Text = carNumbers;
            }
            else
            {
                MessageBox.Show("Введите корректный существующий ID!");
                CarModelTB.Text = string.Empty;
                CarBrandTB.Text = string.Empty;
                CarLicensePlateTB.Text = string.Empty;
                DataFromDB = false;
            }
        }

        // DataBase Control Ends Here
        private void DataGridViewFinder(string searchValue, DataGridView dataGridView)
        {
            for (int i = 0; i <= dataGridView.Rows.Count - 1; i++)
                for (int j = 0; j <= dataGridView.ColumnCount - 1; j++)
                    if (dataGridView.Rows[i].Cells[j].Value != null && dataGridView.Rows[i].Cells[j].Value.ToString() == searchValue)
                        dataGridView.Rows[i].Cells[j].Selected = true;
        }
        private string OwnerFinder(string type)
        {
            if (type == "car")
            {
                string cmd1 = LoadData($"SELECT c.clientSecondName FROM Contract con INNER JOIN Client c ON con.clientID = c.clientID WHERE con.carID = {Convert.ToInt32(CarIdTB.Text)}");
                string cmd2 = LoadData($"SELECT c.clientName FROM Contract con INNER JOIN Client c ON con.clientID = c.clientID WHERE con.carID = {Convert.ToInt32(CarIdTB.Text)}");
                string cmd3 = LoadData($"SELECT c.clientSurName FROM Contract con INNER JOIN Client c ON con.clientID = c.clientID WHERE con.carID = {Convert.ToInt32(CarIdTB.Text)}");
                string uData = $"{cmd2} {cmd1[0]}.{cmd3[0]}";
                return uData;
            }
            else if (type == "ndv")
            {
                string cmd1 = LoadData($"SELECT c.clientSecondName FROM Contract con INNER JOIN Client c ON con.clientID = c.clientID WHERE con.objectID = {Convert.ToInt32(ndvIdTB.Text)}");
                string cmd2 = LoadData($"SELECT c.clientName FROM Contract con INNER JOIN Client c ON con.clientID = c.clientID WHERE con.objectID = {Convert.ToInt32(ndvIdTB.Text)}");
                string cmd3 = LoadData($"SELECT c.clientSurName FROM Contract con INNER JOIN Client c ON con.clientID = c.clientID WHERE con.objectID = {Convert.ToInt32(ndvIdTB.Text)}");
                string uData = $"{cmd2} {cmd1[0]}.{cmd3[0]}";
                return uData;
            }
            return null;
        }
        private void CreateLog(string typeLog, string otherInfo, string times, string user)
        {
            switch (typeLog)
            {
                //CAR BELOW
                //Sensors Below
                case "Hatch": 
                    Logger.WriteLog($"Подкапотное пространство: Срабатывание датчика взлома №{times}. Автомобиль [{otherInfo}].");
                    break;
                case "fDoor": 
                    Logger.WriteLog($"Передние двери: Срабатывание датчика удара №{times}. Автомобиль [{otherInfo}].");
                    break;
                case "rDoor":
                    Logger.WriteLog($"Задние двери: Срабатывание датчика удара №{times}. Автомобиль [{otherInfo}].");
                    break;
                case "Battery":
                    Logger.WriteLog($"АКБ: Аккумулятор разряжен/отключен. Автомобиль [{otherInfo}].");
                    break;
                case "Ignition":
                    Logger.WriteLog($"Зажигание: Двигатель заведен на охране №{times}. Автомобиль [{otherInfo}].");
                    CurrStatusLABEL.Text = "Машина заведена";
                    break;
                case "NoIgnition":
                    Logger.WriteLog($"Зажигание: Попытка включения зажигания №{times}. Автомобиль [{otherInfo}].");
                    break;
                case "Trunk":
                    Logger.WriteLog($"Багажник: Срабатывание датчика удара №{times}. Автомобиль [{otherInfo}].");
                    break;
                case "MultipleSensors": // 3+
                    Logger.WriteLog($"Множественное срабатывание датчиков: [{times}]. Автомобиль [{otherInfo}].");
                    break;
                // Sensors Ends here
                // User actions below
                case "DisableIgnition":
                    Logger.WriteLog($"Глушение связи: Двигатель заглушен сотрудником {user} по обращению владельца договора [{OwnerFinder("car")}]. Автомобиль [{otherInfo}].");
                    break;
                case "EnableIgnition":
                    Logger.WriteLog($"Глушение связи: Доступ к запуску двигателя разрешен сотрудником {user} по обращению владельца договора [{OwnerFinder("car")}]. Автомобиль [{otherInfo}].");
                    break;
                case "DisableSensors":
                    File.WriteAllText(@"log.txt", string.Empty);
                    Logger.WriteLog($"Охрана: Охрана снята сотрудником {user} по обращению владельца договора [{OwnerFinder("car")}]. Автомобиль [{otherInfo}].");
                    break;
                case "EnableSensors":
                    File.WriteAllText(@"log.txt", string.Empty);
                    Logger.WriteLog($"Охрана: Машина поставлена на охрану сотрудником {user} по обращению владельца договора [{OwnerFinder("car")}]. Автомобиль [{otherInfo}].");
                    break;
                case "EmergencyResponseTeam":
                    Logger.WriteLog($"ГБР: Группа Быстрого Реагирования отправлена [{user}]. Автомобиль [{otherInfo}].");
                    break;
                case "GbrArrived":
                    Logger.WriteLog($"ГБР: Группа Быстрого Реагирования прибыла [{user}]. Автомобиль [{otherInfo}].");
                    break;
                // User actions Ends here
                // Car Status Below
                // Car Status End here
                // CAR ENDS HERE
                // NDV BELOW
                case "FireSensor":
                    Logger.WriteLog($"ДИП: Срабатывание комбинированного датчика пожарной сигнализации №{times}. Объект [{otherInfo}].");
                    break;
                case "GasLeaksSensor":
                    Logger.WriteLog($"ДУГ: Срабатывание датчика утечки газа №{times}. Объект [{otherInfo}].");
                    break;
                case "MoveSensor":
                    Logger.WriteLog($"ИК: Срабатывание датчика движения №{times}. Объект [{otherInfo}].");
                    break;
                case "AlarmButton":
                    Logger.WriteLog($"КТС: Срабатывание тревожной кнопки №{times}. Объект [{otherInfo}].");
                    break;
                case "GlassBreakSensor":
                    Logger.WriteLog($"ДРС: Срабатывание датчика разбития стекла №{times}. Объект [{otherInfo}].");
                    break;
                case "WaterLeaksSensor":
                    Logger.WriteLog($"ДУВ: Срабатывание датчика протечки воды №{times}. Объект [{otherInfo}].");
                    break;
                case "DoorOpenSensor":
                    Logger.WriteLog($"СМК: Срабатывание датчика открытия двери №{times}. Объект [{otherInfo}].");
                    break;
                case "NdvDisableSensors":
                    File.WriteAllText(@"log.txt", string.Empty);
                    Logger.WriteLog($"Охрана: Охрана снята сотрудником {user} по обращению владельца договора [{OwnerFinder("ndv")}]. Объект [{otherInfo}].");
                    break;
                case "NdvEnableSensors":
                    File.WriteAllText(@"log.txt", string.Empty);
                    Logger.WriteLog($"Охрана: Объект поставлен на охрану сотрудником {user} по обращению владельца договора [{OwnerFinder("ndv")}]. Объект [{otherInfo}].");
                    break;
                case "NdvEmergencyResponseTeam":
                    Logger.WriteLog($"ГБР: Группа Быстрого Реагирования отправлена [{user}]. Объект [{otherInfo}].");
                    break;
                case "NdvGbrArrived":
                    Logger.WriteLog($"ГБР: Группа Быстрого Реагирования прибыла [{user}]. Объект [{otherInfo}].");
                    break;
                case "MchsSent":
                    Logger.WriteLog($"МЧС: МЧС вызвана на охраняемый объект [{user}]. Объект [{otherInfo}].");
                    break;
                case "MchsArrived":
                    Logger.WriteLog($"МЧС: МЧС вызвана прибыла на охраняемый объект [{user}]. Объект [{otherInfo}].");
                    break;
                    // NDV ENDS HERE

            }
        }
        private void ShowHidePanels(string NamePanel) // Скрывает и показывает панели + сбрасывает значения по умолчанию
        {
            carPanelOpen = false; ndvPanelOpen = false; clientPanelOpen = false; gbrPanelOpen = false; contractPanelOpen = false; alarmPanelOpen = false;
            SensorTriggered = 0; // Сброс значения при переходе
            timerTo = 900;
            DisableIgnition = false; DisableSensors = false; EmergencyResponseTeam = false; DataFromDB = false; MchsSent = false;
            carID = string.Empty; carModel = string.Empty; carBrand = string.Empty; carNumbers = string.Empty;
            File.WriteAllText(@"log.txt", string.Empty);
            // Выше сброс всех значений по умолчанию

            switch (NamePanel)
            {
                case "Car":
                    carPanelOpen = true;
                    CarPANEL.Show();
                    NdvPANEL.Hide();
                    ClientPANEL.Hide();
                    GbrPANEL.Hide();
                    ContractPANEL.Hide();
                    AlarmPANEL.Hide();
                    NdvTablePANEL.Hide();
                    CarTablePANEL.Hide();
                    break;
                case "NDV":
                    ndvPanelOpen = true;
                    CarPANEL.Hide();
                    NdvPANEL.Show();
                    ClientPANEL.Hide();
                    GbrPANEL.Hide();
                    ContractPANEL.Hide();
                    AlarmPANEL.Hide();
                    NdvTablePANEL.Hide();
                    CarTablePANEL.Hide();
                    break;
                case "Client":
                    clientPanelOpen = true;
                    CarPANEL.Hide();
                    NdvPANEL.Hide();
                    ClientPANEL.Show();
                    GbrPANEL.Hide();
                    ContractPANEL.Hide();
                    AlarmPANEL.Hide();
                    NdvTablePANEL.Hide();
                    CarTablePANEL.Hide();
                    break;
                case "GBR":
                    gbrPanelOpen = true;
                    CarPANEL.Hide();
                    NdvPANEL.Hide();
                    ClientPANEL.Hide();
                    GbrPANEL.Show();
                    ContractPANEL.Hide();
                    AlarmPANEL.Hide();
                    NdvTablePANEL.Hide();
                    CarTablePANEL.Hide();
                    break;
                case "Contract":
                    contractPanelOpen = true;
                    CarPANEL.Hide();
                    NdvPANEL.Hide();
                    ClientPANEL.Hide();
                    GbrPANEL.Hide();
                    ContractPANEL.Show();
                    AlarmPANEL.Hide();
                    NdvTablePANEL.Hide();
                    CarTablePANEL.Hide();
                    break;
                case "Alarm":
                    alarmPanelOpen = true;
                    CarPANEL.Hide();
                    NdvPANEL.Hide();
                    ClientPANEL.Hide();
                    GbrPANEL.Hide();
                    ContractPANEL.Hide();
                    AlarmPANEL.Show();
                    break;
                case "All":
                    CarPANEL.Hide();
                    NdvPANEL.Hide();
                    ClientPANEL.Hide();
                    GbrPANEL.Hide();
                    ContractPANEL.Hide();
                    AlarmPANEL.Hide();
                    NdvTablePANEL.Hide();
                    CarTablePANEL.Hide();
                    break;
                case "NdvTable":
                    CarPANEL.Hide();
                    NdvPANEL.Hide();
                    ClientPANEL.Hide();
                    GbrPANEL.Hide();
                    ContractPANEL.Hide();
                    AlarmPANEL.Hide();
                    NdvTablePANEL.Show();
                    CarTablePANEL.Hide();
                    break;
                case "CarTable":
                    CarPANEL.Hide();
                    NdvPANEL.Hide();
                    ClientPANEL.Hide();
                    GbrPANEL.Hide();
                    ContractPANEL.Hide();
                    AlarmPANEL.Hide();
                    NdvTablePANEL.Hide();
                    CarTablePANEL.Show();
                    break;
            }
        }
        private void CarBTN_Click(object sender, EventArgs e) // Авто
        {
            ShowHidePanels("Car");
        }

        private void NDVBTN_Click(object sender, EventArgs e) // НВД
        {
            ShowHidePanels("NDV");
        }

        private void ClientBTN_Click(object sender, EventArgs e) // Клиент
        {
            ShowHidePanels("Client");
        }

        private void GBRBTN_Click(object sender, EventArgs e) // ГБР
        {
            ShowHidePanels("GBR");
        }

        private void ContractBTN_Click(object sender, EventArgs e) // Договор
        {
            ShowHidePanels("Contract");
        }

        private void AlarmBTN_Click(object sender, EventArgs e) // Тревога
        {
            ShowHidePanels("Alarm");
        }

        private void EmergencyCallBTN_Click(object sender, EventArgs e)
        {
            Form operatorcall = new OperatorCall();
            operatorcall.ShowDialog();
        }

        private void AboutProgrammBTN_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Данная программа создана в рамках дипломной работы: \n Информационная система охранного агентства " +
                "\n Создатель: \n Cтудент группы ПКуспк-320 \n Морозов Иван Валерьевич");
        }
        private void DisableRadioButtons(bool disableRBT, string type)
        {
            if (type=="car")
            {
                HoodSensorRBTN.Enabled = !disableRBT;
                FrontDoorSensorRBTN.Enabled = !disableRBT;
                RearDoorSensorRBTN.Enabled = !disableRBT;
                TrunkSensorRBTN.Enabled = !disableRBT;
                BatterySensorRBTN.Enabled = !disableRBT;
                IgnitionRBTN.Enabled = !disableRBT;
                DisableIgnitionSWITCH.Enabled = !disableRBT;
            }
            else if(type=="ndv")
            {
                IkRBTN.Enabled = !disableRBT;
                CmkRBTN.Enabled = !disableRBT;
                DrsRBNT.Enabled = !disableRBT;
                DipRBTN.Enabled = !disableRBT;
                KtsRBTN.Enabled = !disableRBT;
                DyvRBTN.Enabled = !disableRBT;
                DygRBTN.Enabled = !disableRBT;
            }
        }

        // Car Sensors Action below
        private void HoodSensorRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("Hatch", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void FrontDoorSensorRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("fDoor", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void RearDoorSensorRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("rDoor", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void ClientAddNewBTN_Click(object sender, EventArgs e)
        {
            clientBindingSource.AddNew();
        }

        private void ClientUpdateBTN_Click(object sender, EventArgs e)
        {
            clientBindingSource.EndEdit();
            clientTableAdapter.Update(security);
        }

        private void ClientDeleteBTN_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.clientBindingSource.RemoveCurrent();
            this.clientBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(security);
        }

        private void ClientDBCC_Click(object sender, EventArgs e)
        {
            DBCC("Client", "clientID");
        }

        private void StaffAddNewBTN_Click(object sender, EventArgs e)
        {
            staffBindingSource.AddNew();
        }

        private void StaffUpdateBTN_Click(object sender, EventArgs e)
        {
            staffBindingSource.EndEdit();
            staffTableAdapter.Update(security);
        }

        private void StaffDeleteBTN_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.staffBindingSource.RemoveCurrent();
            this.staffBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(security);
        }

        private void StaffDBCCBTN_Click(object sender, EventArgs e)
        {
            DBCC("Staff", "stID");
        }

        private void AlarmAddNewBTN_Click(object sender, EventArgs e)
        {
            triggeringBindingSource.AddNew();
        }

        private void AlarmUpdateBTN_Click(object sender, EventArgs e)
        {
            triggeringBindingSource.EndEdit();
            triggeringTableAdapter.Update(security);
        }

        private void AlarmDeleteBTN_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.triggeringBindingSource.RemoveCurrent();
            this.triggeringBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(security);
        }

        private void AlarmDBCCBTN_Click(object sender, EventArgs e)
        {
            DBCC("Triggering", "tID");
        }

        private void ContractAddNewBTN_Click(object sender, EventArgs e)
        {
            contractBindingSource.AddNew();
        }

        private void ContractUpdateBTN_Click(object sender, EventArgs e)
        {
            contractBindingSource.EndEdit();
            contractTableAdapter.Update(security);
        }

        private void ContractDeleteBTN_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.contractBindingSource.RemoveCurrent();
            this.contractBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(security);
        }

        private void ContractDBCCBTN_Click(object sender, EventArgs e)
        {
            DBCC("Contract", "cID");
        }

        private void NdvToDataBTN_Click(object sender, EventArgs e)
        {
            ShowHidePanels("NdvTable");
        }
        private void NdvToTestBTN_Click(object sender, EventArgs e)
        {
            ShowHidePanels("NDV");
        }

        private void NdvTableAddNewBTN_Click(object sender, EventArgs e)
        {
            objectTableBindingSource.AddNew();
        }

        private void NdvTableUpdateBTN_Click(object sender, EventArgs e)
        {
            objectTableBindingSource.EndEdit();
            objectTableTableAdapter.Update(security);

        }

        private void NdvTableDeleteBTN_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.objectTableBindingSource.RemoveCurrent();
            this.objectTableBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(security);
        }

        private void NdvTableDBCCBTN_Click(object sender, EventArgs e)
        {
            DBCC("ObjectTable", "objectID");
        }

        private void CarTableAddNewBTN_Click(object sender, EventArgs e)
        {
            carBindingSource.AddNew();

        }

        private void CarTableUpdateBTN_Click(object sender, EventArgs e)
        {
            carBindingSource.EndEdit();
            carTableAdapter.Update(security);

        }

        private void CarTableDeleteBTN_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.carBindingSource.RemoveCurrent();
            this.carBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(security);

        }

        private void CarTableDBCCBTN_Click(object sender, EventArgs e)
        {
            DBCC("Car", "CarID");
        }

        private void CarToTestBTN_Click(object sender, EventArgs e)
        {
            ShowHidePanels("Car");

        }

        private void CarSearchBTN_Click(object sender, EventArgs e)
        {
            DataGridViewFinder(CarSearchTB.Text, dataGridView3);  
        }

        private void TriggerSearchBTN_Click(object sender, EventArgs e)
        {
            DataGridViewFinder(TriggerSearchTB.Text, AlarmGridView);
        }

        private void ContractSearchBTN_Click(object sender, EventArgs e)
        {
            DataGridViewFinder(ContractSearchTB.Text, dataGridView1);
        }

        private void StaffSearchBTN_Click(object sender, EventArgs e)
        {
            DataGridViewFinder(StaffSearchTB.Text, staffGridView);
        }

        private void ClientSearch_Click(object sender, EventArgs e)
        {
            DataGridViewFinder(ClientSearchTB.Text, clientDataGridView);
        }

        private void endCarRandomiseBTN_Click(object sender, EventArgs e) // END carRandomiseBTN // Завершить тест (машины)
        {
            LockAllRadBTN(false); // Разблокирует кнопки
            RandSelEnabled = false; // Сбрасывает значение рандомизации по умолчанию
            timerToRand = 0; // Сбрасывает значение таймера рандомизации по умолчанию 
        }

        private void endNdvRandomiseBTN_Click(object sender, EventArgs e) // END carRandomiseBTN // Завершить тест (недвижка)
        {
            LockAllRadBTN(false);
            RandSelEnabled = false;
            timerToRand = 0;
        }

        private void ndvSearchBTN_Click(object sender, EventArgs e)
        {
            DataGridViewFinder(ndvSearchTB.Text, dataGridView2);
        }

        private void startRandomSelectionBTN_Click(object sender, EventArgs e) // START ndvRandomiseBTN // Начать тест (недвижка)
        {
            LockAllRadBTN(true); // Блокирует кнопки
            RandSelEnabled = true; // Начинает рандомизацию
            timerToRand = 150; // Выставляет таймер рандомизации до 150 секунд 
        }

        private void materialButton1_Click(object sender, EventArgs e) // START carRandomiseBTN // Начать тест (машина)
        {
            LockAllRadBTN(true);
            RandSelEnabled = true;
            timerToRand = 150;
        }

        private void CarToTableBTN_Click(object sender, EventArgs e)
        {
            ShowHidePanels("CarTable");

        }

        private void TrunkSensorRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("Trunk", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void CallMchsBTN_Click(object sender, EventArgs e)
        {
            if (MchsSent && timerToMchs != 0)
            {
                Logger.WriteLog($"МЧС уже в пути. Время до прибытия [{timerToMchs / 60} минут]");
            }
            else
            {
                CreateLog("MchsSent", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
                MchsSent = true;
            }

        }

        private void BatterySensorRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("Battery", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void DipRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("FireSensor", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void DygRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("GasLeaksSensor", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void IkRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("MoveSensor", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void EmergencyTeamSentNdvBTN_Click(object sender, EventArgs e)
        {
            if (EmergencyResponseTeam && timerTo != 0)
            {
                Logger.WriteLog($"Группа быстрого реагирования уже в пути. Время до прибытия [{timerTo / 60} минут]");
            }
            else
            {
                CreateLog("NdvEmergencyResponseTeam", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
                NdvTimeToGbrLABEL.Visible = true;
                NdvCurrTimeToGbrLABEL.Visible = true;
                EmergencyResponseTeam = true;
            }
        }

        private void KtsRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("AlarmButton", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
            EmergencyTeamSentNdvBTN_Click(sender, e);
        }

        private void DisableNdvGuardSWITCH_CheckedChanged(object sender, EventArgs e)
        {
            if (DisableSensors)
            {
                NdvCurrStatusLABEL.Text = "Объект поставлен на охрану";
                CreateLog("NdvEnableSensors", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
                DisableSensors = false;
                DisableRadioButtons(DisableSensors, "ndv");
            }
            else
            {
                NdvCurrStatusLABEL.Text = "Объект снят с охраны";
                CreateLog("NdvDisableSensors", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
                DisableSensors = true;
                DisableRadioButtons(DisableSensors, "ndv");
            }
        }

        private void DrsRBNT_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("GlassBreakSensor", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);

        }

        private void DyvRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("WaterLeaksSensor", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);

        }

        private void CmkRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            CreateLog("DoorOpenSensor", $"[{ndvID}] г.{ndvCity} улица {ndvStreet}", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void ndvOkBTN_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ndvIdTB.Text))
            {
                DataFromDB = true;
                ndvCity = LoadData("SELECT objectLocality FROM ObjectTable WHERE objectID='" + ndvIdTB.Text + "'").Replace(" ", "");
                ndvStreet = LoadData("SELECT objectStreet FROM ObjectTable WHERE objectID='" + ndvIdTB.Text + "'").Replace(" ", "");
                ndvHouse = LoadData("SELECT objectHouse FROM ObjectTable WHERE objectID='" + ndvIdTB.Text + "'").Replace(" ", "");
                ndvAppart = LoadData("SELECT objectAppart FROM ObjectTable WHERE objectID='" + ndvIdTB.Text + "'").Replace(" ", "");
                ndvStreet = $"{ndvStreet},{ndvHouse}, кв.{ndvAppart}";
                ndvID = ndvIdTB.Text;
                ndvLocalityTB.Text = ndvCity;
                ndvStreetTB.Text = ndvStreet;
            }
            else
            {
                MessageBox.Show("Введите корректный существующий ID!");
                ndvStreetTB.Text = string.Empty;
                ndvLocalityTB.Text = string.Empty;
                DataFromDB = false;
            }
        }

        private void IgnitionRBTN_Click(object sender, EventArgs e)
        {
            SensorTriggered++;
            if(!DisableIgnition) CreateLog("Ignition", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
            else CreateLog("NoIgnition", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
        }

        private void SentEmergencyBTN_Click(object sender, EventArgs e) // Через 15 минут приедет. Сделать такую фишку в timer.tick
        {
            if (EmergencyResponseTeam && timerTo!=0)
            {
                Logger.WriteLog($"Группа быстрого реагирования уже в пути. Время до прибытия [{timerTo/60} минут]");
            }
            else
            {
                CreateLog("EmergencyResponseTeam", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
                TimeToGbrLABEL.Visible = true;
                CurrTimeToGbrLABEL.Visible = true;
                EmergencyResponseTeam = true;
            }
        }

        private void DisableIgnitionSWITCH_CheckedChanged(object sender, EventArgs e)
        {
            if (DisableIgnition)
            {
                CurrStatusLABEL.Text = "Машина на стоянке";
                CreateLog("EnableIgnition", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
                DisableIgnition = false;
            }
            else
            {
                CurrStatusLABEL.Text = "Двигатель заглушен";
                CreateLog("DisableIgnition", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
                DisableIgnition = true;
            }
        }

        private void DisableSensorsSWITCH_CheckedChanged(object sender, EventArgs e)
        {
            if (DisableSensors)
            {
                CurrStatusLABEL.Text = "Машина поставлена на охрану";
                CreateLog("EnableSensors", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
                DisableSensors = false;
                DisableRadioButtons(DisableSensors, "car");
            }
            else
            {
                CurrStatusLABEL.Text = "Машина снята с охраны";
                CreateLog("DisableSensors", $"[{carID}] {carBrand} {carModel} ({carNumbers})", SensorTriggered.ToString(), UserInfoLABEL.Text);
                DisableSensors = true;
                DisableRadioButtons(DisableSensors, "car");
            }
        }


        // Car Sensors Ends Here
    }
}
