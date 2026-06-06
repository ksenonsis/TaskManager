using System;
using System.Drawing;
using System.Windows.Forms;

namespace TaskManager
{
    // Форма для добавления или редактирования задачи
    public class AddTaskForm : Form
    {
        // Поля формы
        private TextBox txtTitle;
        private TextBox txtDescription;
        private ComboBox cmbStatus;
        private ComboBox cmbPriority;
        private DateTimePicker dtpDueDate;
        private CheckBox chkHasDueDate;
        private Button btnSave;
        private Button btnCancel;

        // Результат — задача
        public TaskItem ResultTask { get; private set; }

        // Режим: редактирование или добавление
        private TaskItem _editTask;

        public AddTaskForm(TaskItem editTask = null)
        {
            _editTask = editTask;
            InitializeComponents();

            // Если редактируем — заполняем поля
            if (_editTask != null)
            {
                this.Text = "Редактировать задачу";
                txtTitle.Text = _editTask.Title;
                txtDescription.Text = _editTask.Description;
                cmbStatus.SelectedItem = _editTask.Status.ToString();
                cmbPriority.SelectedItem = _editTask.Priority.ToString();

                if (_editTask.DueDate.HasValue)
                {
                    chkHasDueDate.Checked = true;
                    dtpDueDate.Value = _editTask.DueDate.Value;
                }
            }
        }

        private void InitializeComponents()
        {
            this.Text = "Новая задача";
            this.Size = new Size(420, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 250);

            // Отступы и шрифт
            Font labelFont = new Font("Segoe UI", 9f, FontStyle.Bold);
            Font inputFont = new Font("Segoe UI", 9.5f);

            int left = 20;
            int y = 20;

            // --- Название ---
            Label lblTitle = new Label();
            lblTitle.Text = "Название задачи:";
            lblTitle.Font = labelFont;
            lblTitle.Location = new Point(left, y);
            lblTitle.AutoSize = true;

            y += 22;
            txtTitle = new TextBox();
            txtTitle.Font = inputFont;
            txtTitle.Location = new Point(left, y);
            txtTitle.Size = new Size(360, 25);

            y += 40;

            // --- Описание ---
            Label lblDesc = new Label();
            lblDesc.Text = "Описание:";
            lblDesc.Font = labelFont;
            lblDesc.Location = new Point(left, y);
            lblDesc.AutoSize = true;

            y += 22;
            txtDescription = new TextBox();
            txtDescription.Font = inputFont;
            txtDescription.Multiline = true;
            txtDescription.Location = new Point(left, y);
            txtDescription.Size = new Size(360, 60);

            y += 75;

            // --- Статус ---
            Label lblStatus = new Label();
            lblStatus.Text = "Статус:";
            lblStatus.Font = labelFont;
            lblStatus.Location = new Point(left, y);
            lblStatus.AutoSize = true;

            cmbStatus = new ComboBox();
            cmbStatus.Font = inputFont;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Location = new Point(left + 120, y - 3);
            cmbStatus.Size = new Size(150, 25);
            foreach (var s in Enum.GetNames(typeof(TaskStatus)))
                cmbStatus.Items.Add(s);
            cmbStatus.SelectedIndex = 0;

            y += 35;

            // --- Приоритет ---
            Label lblPriority = new Label();
            lblPriority.Text = "Приоритет:";
            lblPriority.Font = labelFont;
            lblPriority.Location = new Point(left, y);
            lblPriority.AutoSize = true;

            cmbPriority = new ComboBox();
            cmbPriority.Font = inputFont;
            cmbPriority.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPriority.Location = new Point(left + 120, y - 3);
            cmbPriority.Size = new Size(150, 25);
            foreach (var p in Enum.GetNames(typeof(TaskPriority)))
                cmbPriority.Items.Add(p);
            cmbPriority.SelectedIndex = 1;

            y += 35;

            // --- Дедлайн ---
            chkHasDueDate = new CheckBox();
            chkHasDueDate.Text = "Дедлайн:";
            chkHasDueDate.Font = labelFont;
            chkHasDueDate.Location = new Point(left, y);
            chkHasDueDate.AutoSize = true;
            chkHasDueDate.CheckedChanged += (s, e) => dtpDueDate.Enabled = chkHasDueDate.Checked;

            dtpDueDate = new DateTimePicker();
            dtpDueDate.Font = inputFont;
            dtpDueDate.Location = new Point(left + 120, y - 3);
            dtpDueDate.Size = new Size(150, 25);
            dtpDueDate.Format = DateTimePickerFormat.Short;
            dtpDueDate.MinDate = DateTime.Today;
            dtpDueDate.Enabled = false;

            y += 45;

            // --- Кнопки ---
            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Font = inputFont;
            btnSave.Size = new Size(110, 32);
            btnSave.Location = new Point(left, y);
            btnSave.BackColor = Color.FromArgb(70, 130, 180);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Font = inputFont;
            btnCancel.Size = new Size(90, 32);
            btnCancel.Location = new Point(left + 120, y);
            btnCancel.BackColor = Color.FromArgb(200, 200, 205);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            // Добавляем на форму
            this.Controls.AddRange(new Control[]
            {
                lblTitle, txtTitle,
                lblDesc, txtDescription,
                lblStatus, cmbStatus,
                lblPriority, cmbPriority,
                chkHasDueDate, dtpDueDate,
                btnSave, btnCancel
            });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название задачи!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            // Создаём или обновляем задачу
            ResultTask = _editTask ?? new TaskItem();
            ResultTask.Title = txtTitle.Text.Trim();
            ResultTask.Description = txtDescription.Text.Trim();
            ResultTask.Status = (TaskStatus)Enum.Parse(typeof(TaskStatus), cmbStatus.SelectedItem.ToString());
            ResultTask.Priority = (TaskPriority)Enum.Parse(typeof(TaskPriority), cmbPriority.SelectedItem.ToString());
            ResultTask.DueDate = chkHasDueDate.Checked ? dtpDueDate.Value : (DateTime?)null;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
