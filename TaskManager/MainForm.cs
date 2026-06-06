using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaskManager
{
    public class MainForm : Form
    {
        // Коллекция задач
        private TaskCollection _tasks = new TaskCollection();

        // Элементы интерфейса
        private ListView lvTasks;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnComplete;
        private TextBox txtSearch;
        private Button btnSearch;
        private ComboBox cmbFilter;
        private Label lblStatus;
        private Label lblTitle;

        public MainForm()
        {
            InitializeComponents();
            AddSampleTasks();
            RefreshList();
        }

        private void InitializeComponents()
        {
            // Настройки формы
            this.Text = "📋 Менеджер задач";
            this.Size = new Size(850, 600);
            this.MinimumSize = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 248);

            Font mainFont = new Font("Segoe UI", 9.5f);
            Font boldFont = new Font("Segoe UI", 11f, FontStyle.Bold);

            // --- Заголовок ---
            lblTitle = new Label();
            lblTitle.Text = "📋 Менеджер задач";
            lblTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 60, 120);
            lblTitle.Location = new Point(15, 12);
            lblTitle.AutoSize = true;

            // --- Панель поиска и фильтра ---
            Label lblSearch = new Label();
            lblSearch.Text = "Поиск:";
            lblSearch.Font = mainFont;
            lblSearch.Location = new Point(15, 55);
            lblSearch.AutoSize = true;

            txtSearch = new TextBox();
            txtSearch.Font = mainFont;
            txtSearch.Location = new Point(65, 52);
            txtSearch.Size = new Size(200, 25);
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) RefreshList(); };

            btnSearch = new Button();
            btnSearch.Text = "🔍 Найти";
            btnSearch.Font = mainFont;
            btnSearch.Location = new Point(275, 50);
            btnSearch.Size = new Size(85, 28);
            btnSearch.BackColor = Color.FromArgb(70, 130, 180);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => RefreshList();

            Label lblFilter = new Label();
            lblFilter.Text = "Фильтр:";
            lblFilter.Font = mainFont;
            lblFilter.Location = new Point(380, 55);
            lblFilter.AutoSize = true;

            cmbFilter = new ComboBox();
            cmbFilter.Font = mainFont;
            cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilter.Location = new Point(435, 52);
            cmbFilter.Size = new Size(140, 25);
            cmbFilter.Items.Add("Все задачи");
            foreach (var s in Enum.GetNames(typeof(TaskStatus)))
                cmbFilter.Items.Add(s);
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => RefreshList();

            // Кнопка сброса поиска
            Button btnReset = new Button();
            btnReset.Text = "✕ Сброс";
            btnReset.Font = mainFont;
            btnReset.Location = new Point(585, 50);
            btnReset.Size = new Size(80, 28);
            btnReset.BackColor = Color.FromArgb(200, 200, 205);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += (s, e) => { txtSearch.Clear(); cmbFilter.SelectedIndex = 0; RefreshList(); };

            // --- ListView ---
            lvTasks = new ListView();
            lvTasks.Font = mainFont;
            lvTasks.Location = new Point(15, 90);
            lvTasks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvTasks.Size = new Size(this.ClientSize.Width - 180, this.ClientSize.Height - 130);
            lvTasks.View = View.Details;
            lvTasks.FullRowSelect = true;
            lvTasks.GridLines = true;
            lvTasks.MultiSelect = false;
            lvTasks.BackColor = Color.White;
            lvTasks.BorderStyle = BorderStyle.FixedSingle;

            // Столбцы
            lvTasks.Columns.Add("ID", 40);
            lvTasks.Columns.Add("Название", 220);
            lvTasks.Columns.Add("Приоритет", 85);
            lvTasks.Columns.Add("Статус", 100);
            lvTasks.Columns.Add("Дедлайн", 90);
            lvTasks.Columns.Add("Создана", 90);

            lvTasks.DoubleClick += (s, e) => EditSelectedTask();

            // --- Боковая панель кнопок ---
            int btnX = this.ClientSize.Width - 145;
            int btnY = 90;
            int btnW = 130;
            int btnH = 35;
            int btnGap = 12;

            btnAdd = MakeButton("➕ Добавить", Color.FromArgb(60, 160, 90), mainFont);
            btnAdd.Location = new Point(btnX, btnY);
            btnAdd.Size = new Size(btnW, btnH);
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Click += (s, e) => AddTask();

            btnY += btnH + btnGap;
            btnEdit = MakeButton("✏️ Изменить", Color.FromArgb(70, 130, 180), mainFont);
            btnEdit.Location = new Point(btnX, btnY);
            btnEdit.Size = new Size(btnW, btnH);
            btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEdit.Click += (s, e) => EditSelectedTask();

            btnY += btnH + btnGap;
            btnComplete = MakeButton("✅ Выполнена", Color.FromArgb(90, 160, 120), mainFont);
            btnComplete.Location = new Point(btnX, btnY);
            btnComplete.Size = new Size(btnW, btnH);
            btnComplete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnComplete.Click += (s, e) => MarkComplete();

            btnY += btnH + btnGap;
            btnDelete = MakeButton("🗑️ Удалить", Color.FromArgb(190, 70, 70), mainFont);
            btnDelete.Location = new Point(btnX, btnY);
            btnDelete.Size = new Size(btnW, btnH);
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Click += (s, e) => DeleteSelectedTask();

            // --- Строка статуса ---
            lblStatus = new Label();
            lblStatus.Font = new Font("Segoe UI", 9f);
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Location = new Point(15, this.ClientSize.Height - 30);
            lblStatus.AutoSize = true;
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            // Добавляем всё на форму
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblSearch, txtSearch, btnSearch,
                lblFilter, cmbFilter, btnReset,
                lvTasks,
                btnAdd, btnEdit, btnComplete, btnDelete,
                lblStatus
            });

            this.Resize += (s, e) => RepositionButtons();
        }

        // Вспомогательный метод создания кнопки
        private Button MakeButton(string text, Color color, Font font)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Font = font;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        // Перемещает кнопки при изменении размера окна
        private void RepositionButtons()
        {
            int btnX = this.ClientSize.Width - 145;
            int btnY = 90;
            int btnH = 35;
            int btnGap = 12;
            int btnW = 130;

            lvTasks.Size = new Size(this.ClientSize.Width - 180, this.ClientSize.Height - 130);

            btnAdd.Location = new Point(btnX, btnY); btnAdd.Size = new Size(btnW, btnH);
            btnY += btnH + btnGap;
            btnEdit.Location = new Point(btnX, btnY); btnEdit.Size = new Size(btnW, btnH);
            btnY += btnH + btnGap;
            btnComplete.Location = new Point(btnX, btnY); btnComplete.Size = new Size(btnW, btnH);
            btnY += btnH + btnGap;
            btnDelete.Location = new Point(btnX, btnY); btnDelete.Size = new Size(btnW, btnH);

            lblStatus.Location = new Point(15, this.ClientSize.Height - 30);
        }

        // --- Действия ---

        private void AddTask()
        {
            using (var form = new AddTaskForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _tasks.Add(form.ResultTask);
                    RefreshList();
                }
            }
        }

        private void EditSelectedTask()
        {
            var task = GetSelectedTask();
            if (task == null) return;

            using (var form = new AddTaskForm(task))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    RefreshList();
            }
        }

        private void DeleteSelectedTask()
        {
            var task = GetSelectedTask();
            if (task == null) return;

            var result = MessageBox.Show(
                $"Удалить задачу «{task.Title}»?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _tasks.Remove(task.Id);
                RefreshList();
            }
        }

        private void MarkComplete()
        {
            var task = GetSelectedTask();
            if (task == null) return;

            task.Status = TaskStatus.Выполнена;
            RefreshList();
        }

        // Получить выбранную задачу
        private TaskItem GetSelectedTask()
        {
            if (lvTasks.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите задачу из списка.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            int id = (int)lvTasks.SelectedItems[0].Tag;
            return _tasks.GetById(id);
        }

        // Обновить список задач
        private void RefreshList()
        {
            lvTasks.Items.Clear();

            // Получаем задачи с учётом поиска
            List<TaskItem> list = _tasks.Search(txtSearch.Text);

            // Фильтр по статусу
            if (cmbFilter.SelectedIndex > 0)
            {
                var filterStatus = (TaskStatus)(cmbFilter.SelectedIndex - 1);
                list = list.FindAll(t => t.Status == filterStatus);
            }

            foreach (var task in list)
            {
                var item = new ListViewItem(task.Id.ToString());
                item.SubItems.Add(task.Title);
                item.SubItems.Add(task.Priority.ToString());
                item.SubItems.Add(task.Status.ToString());
                item.SubItems.Add(task.DueDate.HasValue ? task.DueDate.Value.ToString("dd.MM.yyyy") : "—");
                item.SubItems.Add(task.CreatedDate.ToString("dd.MM.yyyy"));
                item.Tag = task.Id;

                // Цвет строки по статусу
                if (task.Status == TaskStatus.Выполнена)
                    item.ForeColor = Color.Gray;
                else if (task.Priority == TaskPriority.Высокий)
                    item.ForeColor = Color.DarkRed;
                else if (task.Priority == TaskPriority.Низкий)
                    item.ForeColor = Color.DarkGreen;

                lvTasks.Items.Add(item);
            }

            lblStatus.Text = $"Всего задач: {_tasks.Count}  |  Отображается: {list.Count}";
        }

        // Добавляем демо-задачи при запуске
        private void AddSampleTasks()
        {
            _tasks.Add(new TaskItem
            {
                Title = "Сдать отчёт по практике",
                Description = "Написать и сдать отчёт преподавателю",
                Priority = TaskPriority.Высокий,
                Status = TaskStatus.ВРаботе,
                DueDate = DateTime.Today.AddDays(3)
            });
            _tasks.Add(new TaskItem
            {
                Title = "Прочитать главу по C#",
                Description = "Глава 5 — Классы и объекты",
                Priority = TaskPriority.Средний,
                Status = TaskStatus.Новая,
                DueDate = DateTime.Today.AddDays(7)
            });
            _tasks.Add(new TaskItem
            {
                Title = "Купить продукты",
                Description = "Молоко, хлеб, яйца",
                Priority = TaskPriority.Низкий,
                Status = TaskStatus.Выполнена
            });
        }
    }
}
