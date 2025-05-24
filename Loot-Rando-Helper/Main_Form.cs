using BL2;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace BL2_Test_Loot_Rando
{
    public partial class Main_Form : Form
    {
        public const string beta_version = " BETA 7 ";
        public const string build_date = " (built Jan 18, 2025) ";

        static readonly HttpClient http_client = new HttpClient();
        
        private static string filename_regex = @"([a-zA-Z0-9]{5})-([a-zA-Z0-9]{5})-([a-zA-Z0-9]{5})\.txt";

        public string text_to_search = String.Empty;

        public BL2_Data? bl2 = null;
        public List<string>? bl2_steam_folders;
        public string bl2_root_folder = String.Empty;
        public string bl2_seeds_folder = String.Empty;
        public string bl2_selected_seed_full_path = String.Empty;

        public Char_Class currently_selected_char_class;

        public Color selected_highlite_color = SystemColors.ActiveCaption;

        public Found_Type weap_found_type = Found_Type.EITHER;
        public Found_Type item_found_type = Found_Type.EITHER;
        public Found_Type hint_found_type = Found_Type.EITHER;
        public Found_Type area_found_type = Found_Type.EITHER;


        // --------------------------------------------------------------------------------------------------------------------------------------------
        public Main_Form()
        {
            InitializeComponent();
            
            if (OperatingSystem.IsWindows())
            {
                Icon = Properties.Resources.loot_rando;
            }

            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, seed_dgv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, weap_dgv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, item_dgv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, area_dgv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, hint_dgv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, hint_dgv_2, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, meo_search_dgv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, loot_search_dgv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dupe_dgv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, area_maps_lv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, area_names_lv, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, seed_tlp, [true]);
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, six_life_tlp, [true]);


            seed_dgv.Tag = new Sorting_Data() { dgv = seed_dgv, col_name = "date_modified_col", col_sorting_list = [] };
            weap_dgv.Tag = new Sorting_Data() { dgv = weap_dgv, col_name = "weap_col_name", col_sorting_list = [] };
            item_dgv.Tag = new Sorting_Data() { dgv = item_dgv, col_name = "item_col_name", col_sorting_list = [] };
            area_dgv.Tag = new Sorting_Data() { dgv = area_dgv, col_name = "area_col_name", col_sorting_list = [] };
            hint_dgv.Tag = new Sorting_Data() { dgv = hint_dgv, col_name = "hint_col_name", col_sorting_list = [] };
            hint_dgv_2.Tag = new Sorting_Data() { dgv = hint_dgv_2, col_name = "hint2_col_name", col_sorting_list = [] };
            loot_search_dgv.Tag = new Sorting_Data() { dgv = loot_search_dgv, col_name = "loot_search_name_col", col_sorting_list = [] };
            meo_search_dgv.Tag = new Sorting_Data() { dgv = meo_search_dgv, col_name = "meo_search_name_col", col_sorting_list = [] };
            dupe_dgv.Tag = new Sorting_Data() { dgv = dupe_dgv, col_name = "dupe_col_name", col_sorting_list = [] };

            vh_zero_button.Tag = (int)Char_Class.ZERO;
            vh_maya_button.Tag = (int)Char_Class.MAYA;
            vh_krieg_button.Tag = (int)Char_Class.KRIEG;
            vh_sal_button.Tag = (int)Char_Class.SAL;
            vh_gaige_button.Tag = (int)Char_Class.GAIGE;
            vh_axton_button.Tag = (int)Char_Class.AXTON;
            vh_all_button.Tag = (int)Char_Class.NOT_SET;
            vh_all_button.BackColor = selected_highlite_color;

            weap_ar_button.Tag = Loot_Type.AR;
            weap_pistol_button.Tag = Loot_Type.PISTOL;
            weap_shotgun_button.Tag = Loot_Type.SHOTGUN;
            weap_launcher_button.Tag = Loot_Type.LAUNCHER;
            weap_smg_button.Tag = Loot_Type.SMG;
            weap_sniper_button.Tag = Loot_Type.SNIPER;
            weap_all_button.Tag = Loot_Type.ALL_WEAP;
            weap_all_button.BackColor = selected_highlite_color;

            item_com_button.Tag = Loot_Type.COM;
            item_relic_button.Tag = Loot_Type.RELIC;
            item_grenade_button.Tag = Loot_Type.GRENADE;
            item_shield_button.Tag = Loot_Type.SHIELD;
            item_all_button.Tag = Loot_Type.ALL_ITEM;
            item_all_button.BackColor = selected_highlite_color;

            weap_show_all_rb.Tag = Found_Type.EITHER;
            weap_show_found_rb.Tag = Found_Type.FOUND;
            weap_show_not_found_rb.Tag = Found_Type.NOT_FOUND;

            item_show_all_rb.Tag = Found_Type.EITHER;
            item_show_found_rb.Tag = Found_Type.FOUND;
            item_show_not_found_rb.Tag = Found_Type.NOT_FOUND;

            hint_show_all_rb.Tag = Found_Type.EITHER;
            hint_show_found_rb.Tag = Found_Type.FOUND;
            hint_show_not_found_rb.Tag = Found_Type.NOT_FOUND;

            area_show_all_rb.Tag = Found_Type.EITHER;
            area_show_found_rb.Tag = Found_Type.FOUND;
            area_show_not_found_rb.Tag = Found_Type.NOT_FOUND;

            seed_rtb_1.Text = String.Empty;
            seed_rtb_2.Text = String.Empty;
            seed_rtb_3.Text = String.Empty;

            seed_col_locations.HeaderText = "Total\nLocations";
            seed_col_items.HeaderText = "Weapons and Items\nFound | Total | % ";
            seed_col_nothings.HeaderText = "Duplicates or Nothings\nFound | Total | %";
            seed_col_hints.HeaderText = "Hints\nFound";

            weap_toggle_totals_button_Click(null, null);
            item_toggle_totals_button_Click(null, null);

            seed_file_dlg.InitialDirectory = AssemblyDirectory;

            hint_comboBox.Items.AddRange(Seed_Line.hints.ToArray());

            for (int x = 1; x < 4; x++)
                for (int y = 1; y < 7; y++)
                {
                    Label label = six_life_tlp.GetControlFromPosition(x, y) as Label;
                    label.Text = String.Format("{0}, {1}", x, y);
                }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private async void load_web_seed_button_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(web_seed_tb.Text) || !web_seed_tb.Text.StartsWith("https://gist.github.com"))
                return;

            string html_text = String.Empty;
            try
            {
                html_text = await http_client.GetStringAsync(web_seed_tb.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Had an error\n\n{0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!String.IsNullOrEmpty(html_text))
            {
                var parts = html_text.Split(".txt", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    var match = Regex.Match(parts[1].Substring(parts[1].LastIndexOf("<a href=")), @"(\/raw\/[a-f0-9]{40}\/[a-z0-9-]+)");
                    if (match.Success)
                    {
                        string? data = match.Groups[1].Value;
                        if (data != null)
                        {
                            string download_link = String.Format("{0}{1}.txt", web_seed_tb.Text, data);
                            string path_to_exe = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath);
                            string local_file_full_path = Path.Combine(path_to_exe, "web seeds", Path.GetFileName(download_link));

                            using var stream = await http_client.GetStreamAsync(new Uri(download_link));
                            using var file_stream = new FileStream(local_file_full_path, FileMode.Create);
                            await stream.CopyToAsync(file_stream);

                            Thread.Sleep(1000);
                            Add_Seed_To_DGV(local_file_full_path);
                        }
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void six_life_random_button_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            List<Six_Life_Char> chars =
            [
                new Six_Life_Char() { name = "Gaige" },
                new Six_Life_Char() { name = "Axton" },
                new Six_Life_Char() { name = "Zero" },
                new Six_Life_Char() { name = "Krieg" },
                new Six_Life_Char() { name = "Sal" },
                new Six_Life_Char() { name = "Maya" },
            ];

            List<Six_Life_Seed> seeds =
            [
                new() { name = "SEED 1", color = Color.Red },
                new() { name = "SEED 2", color = Color.Green },
                new() { name = "SEED 3", color = Color.Blue },
                new() { name = "SEED 4", color = Color.DarkOrange },
                new() { name = "SEED 5", color = Color.Brown },
                new() { name = "SEED 6", color = Color.Black },
            ];

            int abort_check = 0;

            for (int playthru = 0; playthru < 3; playthru++)
            {
                List<int> seed_indexes = [0, 1, 2, 3, 4, 5];

                for (int char_index = 0; char_index < 6; char_index++)
                {
                    Six_Life_Char slc = chars[char_index];

                    while (slc.seed_numbers.Count < playthru + 1)
                    {
                        if (++abort_check > 1000)
                            return;

                        int v = seed_indexes[rand.Next(seed_indexes.Count)];
                        seed_indexes.Remove(v);

                        if (slc.seed_numbers.Contains(v) == false)
                            slc.seed_numbers.Add(v);
                        else
                            seed_indexes.Add(v);
                    }
                }
            }

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 6; y++)
                {
                    var seed = seeds[chars[y].seed_numbers[x]];
                    chars[y].seeds.Add(seed);

                    Label label = six_life_tlp.GetControlFromPosition(x + 1, y + 1) as Label;
                    label.Text = seed.name;
                    label.ForeColor = seed.color;
                }

        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void Main_Form_Load(object sender, EventArgs e)
        {
            if (OperatingSystem.IsWindows())
                bl2_steam_folders = Find_Games.Search_Registry_For_Steam_BL2();
            else
            {
                bl2_steam_folders = new List<string>();
                bl2_steam_folders.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.local/share/Steam/steamapps/common");
            }
            if (bl2_steam_folders.Count < 1)
                steam_library_folders_rtb.Text = "No Steam libraries found.";
            else
                Populate_Seeds();
            bl2 = null;
            try
            {
                bl2 = JsonConvert.DeserializeObject<BL2_Data>(File.ReadAllText("bl2_v2.json"));
                if (bl2 != null)
                {
                    Loot.bl2 = bl2;
                    Meo.bl2 = bl2;
                    Map.bl2 = bl2;

                    BL2_Data.char_class = currently_selected_char_class;
                    bl2.Update_Lists_and_Totals();

                    Show_All_Data();
                }
                else
                    MessageBox.Show("Error - found bl2_v2.json but it's NULL");
            }
            catch (FileNotFoundException exc) 
            {
                MessageBox.Show(
@"Error - bl2_v2.json not found. 

Download it from:
https://github.com/NickPoirier/bl2.json/raw/refs/heads/main/bl2_v2.json

And put into program's folder:" + "\n" + exc.FileName);
                
            }
            catch (Exception)
            {
                MessageBox.Show("Error - Reading bl2_v2.json failed.\nHint: Make sure both it and Helper are using up-to-date versions");
                throw;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void search_rtb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tc.SelectedTab != search_tp)
                {
                    tc.SelectedTab = search_tp;
                    search_rtb.Select();
                }

                text_to_search = Regex.Replace(search_rtb.Text, @"\t|\n|\r", "");
                search_rtb.Text = String.Empty;
                Do_Search();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Do_Search()
        {
            if (bl2 == null)
                return;

            loot_search_dgv.Rows.Clear();
            meo_search_dgv.Rows.Clear();

            last_search_label.Text = text_to_search;

            if (!String.IsNullOrEmpty(text_to_search))
            {
                var results = bl2.Search_MEO(text_to_search);
                foreach (var row in results)
                    meo_search_dgv.Rows.Add(row);

                results = bl2.Search_Loot(text_to_search);
                foreach (var row in results)
                    loot_search_dgv.Rows.Add(row);

                meo_search_dgv.ClearSelection();
                meo_search_dgv.CurrentCell = null;

                loot_search_dgv.ClearSelection();
                loot_search_dgv.CurrentCell = null;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void search_rtb_MouseDown(object sender, MouseEventArgs e)
        {
            search_rtb.Text = String.Empty;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                tc.SelectedTab = search_tp;
                search_rtb.Select();
                search_rtb.Text = String.Empty;

                return true;  // key is handled, do nothing further with it
            }

            return false;  // key NOT handled here, pass to underlying control
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void vh_button_Click(object sender, EventArgs e)
        {
            Button clicked_button = sender as Button;

            if (clicked_button != null)
            {
                if (clicked_button.BackColor == selected_highlite_color)
                    return;

                BL2_Data.char_class = (Char_Class)((int)clicked_button.Tag);

                foreach (Button b in vh_flow_layout_panel.Controls)
                    b.BackColor = b == clicked_button ? selected_highlite_color : SystemColors.Control;

                weap_dgv.Rows.Clear();
                item_dgv.Rows.Clear();
                area_dgv.Rows.Clear();
                area_maps_lv.Items.Clear();
                area_names_lv.SelectedItems.Clear();
                hint_dgv.Rows.Clear();
                hint_dgv_2.ClearSelection();

                Show_All_Data();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void seed_load_button_Click(object sender, EventArgs e)
        {
            if (seed_dgv.SelectedRows.Count > 0)
            {
                Seed seed = (Seed)(seed_dgv.SelectedRows[0].Tag);
                if (seed != null && bl2 != null)
                {
                    bl2.Load_Seed(seed);
                    Text = String.Format("Loot Rando Helper{0}{1}  Seed: {2}", beta_version, build_date, seed.file_name);
                    Show_All_Data();

                    if (seed.total_hints == 0 && tc.TabPages.Contains(hint_tp))
                        tc.TabPages.Remove(hint_tp);
                    else if (seed.total_hints > 0 && !tc.TabPages.Contains(hint_tp))
                        tc.TabPages.Insert(4, hint_tp);


                    selected_seed_rtb.Text = seed.fi.FullName;
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void seed_browse_button_Click(object sender, EventArgs e)
        {
            var answer = seed_file_dlg.ShowDialog();
            if (answer == DialogResult.OK)
            {
                var match = Regex.Match(Path.GetFileName(seed_file_dlg.FileName), filename_regex);
                if (match.Success)
                {
                    Add_Seed_To_DGV(seed_file_dlg.FileName);

                    seed_dgv.ClearSelection();
                    seed_dgv.CurrentCell = null;

                    if (seed_dgv.Rows.Count > 0)
                        seed_dgv.CurrentCell = seed_dgv.Rows[seed_dgv.Rows.Count - 1].Cells[0];
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public Loot_Type Get_Selected_Loot_Type(Panel button_panel)
        {
            foreach (Control c in button_panel.Controls)
                if (c.BackColor == selected_highlite_color && c.GetType() == typeof(Button))
                    return (Loot_Type)c.Tag;

            return Loot_Type.NONE;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Show_All_Data()
        {
            if (bl2 == null) return;

            Show_Selected_Weapon_Type();
            Show_Selected_Item_Type();
            Show_Areas();
            Show_Hints();
            Show_Dupes_Or_Nothings();

            Update_Loot_Totals();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Update_Loot_Totals()
        {
            if (bl2 == null) return;

            for (int i = 0; i < 5; i++)
            {
                Loot_Type type = (Loot_Type)(i + 2);
                all_item_lv.Items[i].SubItems.Clear();
                all_item_lv.Items[i].Text = BL2_Data.type_string[type];
                all_item_lv.Items[i].SubItems.AddRange(bl2.Get_Loot_Type_Details_Row(type));
            }

            for (int i = 0; i < 7; i++)
            {
                Loot_Type type = (Loot_Type)(i + 7);
                all_weap_lv.Items[i].SubItems.Clear();
                all_weap_lv.Items[i].Text = BL2_Data.type_string[type];
                all_weap_lv.Items[i].SubItems.AddRange(bl2.Get_Loot_Type_Details_Row(type));
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Show_Selected_Weapon_Type()
        {
            if (bl2 == null) return;

            Loot_Type type = Get_Selected_Loot_Type(weap_panel);

            weap_dgv.Rows.Clear();
            foreach (var loot in bl2.loot_lists[type])
                foreach (var row in loot.Get_Source_Rows(weap_found_type, type, false))
                    weap_dgv.Rows.Add(row);

            weap_dgv.CurrentCell = null;
            weap_dgv.ClearSelection();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Show_Selected_Item_Type()
        {
            if (bl2 == null) return;

            Loot_Type type = Get_Selected_Loot_Type(item_panel);

            item_dgv.Rows.Clear();
            foreach (var loot in bl2.loot_lists[type])
                foreach (var row in loot.Get_Source_Rows(item_found_type, type, false))
                    item_dgv.Rows.Add(row);

            item_dgv.CurrentCell = null;
            item_dgv.ClearSelection();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Show_Areas()
        {
            if (bl2 == null) return;

            area_names_lv.Items.Clear();
            area_maps_lv.Items.Clear();
            area_dgv.Rows.Clear();

            ListViewItem lvi = new ListViewItem() { Text = "Everything" };
            lvi.SubItems.AddRange(bl2.Everything_MEO_Detail_Row());
            area_names_lv.Items.Add(lvi);

            foreach (var area in bl2.areas)
            {
                lvi = new ListViewItem() { Text = area.name };
                lvi.SubItems.AddRange(area.MEO_Detail_Row());
                lvi.Tag = area;
                area_names_lv.Items.Add(lvi);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Show_Maps_For_Selected_Area()
        {
            if (bl2 == null || area_names_lv.SelectedIndices.Count < 1) return;

            area_maps_lv.Items.Clear();
            area_dgv.Rows.Clear();

            if (area_names_lv.SelectedIndices[0] == 0)
            {
                // EVERYTHING
                foreach (var area in bl2.areas)
                    foreach (var map in area.maps)
                        Add_Map_To_ListView(map);

            }
            else
            {
                Area area = area_names_lv.Items[area_names_lv.SelectedIndices[0]].Tag as Area;

                if (area != null && area.maps != null)
                    foreach (var map in area.maps)
                        Add_Map_To_ListView(map);
            }

            if (area_maps_lv.Items.Count == 1)
                area_maps_lv.Items[0].Selected = true;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Add_Map_To_ListView(Map map)
        {
            ListViewItem lvi = new ListViewItem() { Text = map.name };
            lvi.SubItems.AddRange(map.Map_MEO_Detail_Row());
            lvi.Tag = map;
            area_maps_lv.Items.Add(lvi);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Show_MEO_For_Selected_Map()
        {
            if (bl2 == null || area_maps_lv.SelectedIndices.Count < 1) return;

            Map map = area_maps_lv.Items[area_maps_lv.SelectedIndices[0]].Tag as Map;

            area_map_name_label.Text = String.Format("Map: {0}", map.name);

            if (map != null && map.meos != null)
            {
                area_dgv.Rows.Clear();
                foreach (var meo in map.Get_MEO_List(area_found_type))
                    area_dgv.Rows.Add(meo.Get_Detail_Row());
            }

            area_dgv.ClearSelection();
            area_dgv.CurrentCell = null;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void area_maps_lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            Show_MEO_For_Selected_Map();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void area_names_lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            Show_Maps_For_Selected_Area();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Show_Hints()
        {
            if (bl2 == null) return;

            hint_dgv.Rows.Clear();
            hint_dgv_2.Rows.Clear();

            if (bl2.seed != null)
                foreach (var sl in bl2.seed.all_seed_lines)
                    if (sl.status == Seed_Line_Status.Hint)
                        hint_dgv_2.Rows.Add(sl.Get_Hint_Row());

            hint_total_label.Text = String.Format("Total hints found: {0}", hint_dgv_2.Rows.Count);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Show_Dupes_Or_Nothings()
        {
            if (bl2 == null || bl2.seed == null) return;

            dupe_dgv.Rows.Clear();

            foreach (DataGridViewColumn col in dupe_dgv.Columns)
                col.Visible = true;

            // DUPES
            if (bl2.seed.options.misc["Duplicate Items"] == true)
            {
                dupe_description_label.Text = "Total Duplicates: " + bl2.seed.duplicates.Count;
                dupes_tp.Text = "Duplicates";

                foreach (var seed_line in bl2.seed.duplicates)
                    foreach (var row in seed_line.loot.Get_Source_Rows(Found_Type.EITHER, Loot_Type.ANY, true))
                        dupe_dgv.Rows.Add(row);
            }
            // NOTHINGS
            else
            {
                dupe_description_label.Text = "Total Nothings: " + bl2.seed.nothings.Count;
                dupes_tp.Text = "Nothings";

                dupe_col_rarity.Visible = false;
                dupe_col_hint.Visible = false;
                dupe_col_rate.Visible = false;
                dupe_col_options.Visible = false;

                foreach (var seed_line in bl2.seed.nothings)
                    dupe_dgv.Rows.Add(seed_line.meo.Get_Nothing_Row());
            }

            dupe_dgv.Tag = new Sorting_Data() { dgv = dupe_dgv, col_name = "dupe_col_name", col_sorting_list = [] };
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void dgv_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new();

                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem() { Text = column.HeaderText, Checked = column.Visible };

                    if (item.Text != "Name")
                        item.Click += (obj, ea) =>
                        {
                            column.Visible = !item.Checked;
                            item.Checked = column.Visible;
                            menu.Show(dgv, e.Location);
                        };

                    menu.Items.Add(item);
                }

                menu.Show(dgv, e.Location);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void dgv_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DataGridView dgv = sender as DataGridView;
                DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
                if (hit.Type == DataGridViewHitTestType.None)
                {
                    dgv.ClearSelection();
                    dgv.CurrentCell = null;
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null)
            {
                Sorting_Data sd = dgv.Tag as Sorting_Data;
                if (sd != null)
                    Sort_DGV(sd, e.ColumnIndex);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private static void Sort_DGV(Sorting_Data sd, int col_index)
        {
            var sort_col = sd.col_sorting_list.Find(o => o.ColumnIndex == col_index);
            if (sort_col == null)
            {
                sort_col = new ColumnSorting() { ColumnIndex = col_index, Direction = ListSortDirection.Ascending };
                sd.col_sorting_list.Add(sort_col);
            }
            else
                sort_col.Direction = sort_col.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            DataGridViewColumn column_clicked = sd.dgv.Columns[col_index];
            if (column_clicked.SortMode == DataGridViewColumnSortMode.Programmatic)
            {
                MyTwoColumnComparer Sort2C = new MyTwoColumnComparer(
                    column_clicked.Name, sort_col.Direction == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending, sd.col_name, SortOrder.Ascending
                );
                sd.dgv.Sort(Sort2C);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Populate_Seeds()
        {
            foreach (string path in bl2_steam_folders)
            {
                steam_library_folders_rtb.Text += path + Environment.NewLine;

                if (String.IsNullOrEmpty(bl2_root_folder))
                {
                    var sub_folders = Directory.GetDirectories(path, "Borderlands 2", SearchOption.TopDirectoryOnly);
                    if (sub_folders.Length > 0)
                    {
                        bl2_root_folder = sub_folders[0];
                        bl2_folder_rtb.Text = bl2_root_folder;

                        string seeds_path = "";
                        if(OperatingSystem.IsWindows())
                            seeds_path = bl2_root_folder + @"\Binaries\Win32\Mods\LootRandomizer\Seeds";
                        else
                            seeds_path = bl2_root_folder + @"/sdk_mods/LootRandomizer/Seeds";
                        if (Path.Exists(seeds_path))
                        {
                            bl2_seeds_folder = seeds_path;
                            seeds_folder_rtb.Text = seeds_path;

                            var seed_files = Directory.GetFiles(seeds_path, "*.txt", SearchOption.TopDirectoryOnly);
                            foreach (string full_file_path in seed_files)
                            {
                                Match match = Regex.Match(Path.GetFileName(full_file_path), filename_regex);
                                if (match.Success)
                                    Add_Seed_To_DGV(full_file_path);
                            }
                        }
                    }
                }
            }

            if (seed_dgv.Rows.Count > 0)
                seed_dgv.CurrentCell = seed_dgv.Rows[seed_dgv.Rows.Count - 1].Cells[0];
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Add_Seed_To_DGV(string full_file_path)
        {
            int retry = 0;
            Seed? seed = null;

            do
            {
                seed = new Seed(full_file_path);
                
                if (seed != null && !String.IsNullOrEmpty(seed.error_msg))
                    Thread.Sleep(1000);

            } while (retry++ < 5 && (seed != null && String.IsNullOrEmpty(seed.error_msg)));
            
            if (seed != null && String.IsNullOrEmpty(seed.error_msg))
            {
                object[] cells = seed.Get_Details_Row();
                seed_dgv.Rows.Add(cells);
                seed_dgv.Rows[seed_dgv.Rows.Count - 1].Tag = seed;

                seed_dgv.ClearSelection();
                seed_dgv.CurrentCell = null;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void seed_dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (seed_dgv.SelectedRows.Count > 0)
            {
                Seed seed = (Seed)(seed_dgv.SelectedRows[0].Tag);

                if (seed != null)
                {
                    seed_rtb_1.Text = Environment.NewLine;
                    foreach (var option in seed.options.areas)
                        Add_Options_Text(seed_rtb_1, String.Format("  {1,3} - {0}{2}", option.Key, option.Value ? "ON" : "OFF", Environment.NewLine), option.Value);

                    seed_rtb_2.Text = Environment.NewLine;
                    foreach (var option in seed.options.mission_enemy)
                        Add_Options_Text(seed_rtb_2, String.Format("  {1,3} - {0}{2}", option.Key, option.Value ? "ON" : "OFF", Environment.NewLine), option.Value);

                    seed_rtb_3.Text = Environment.NewLine;
                    foreach (var option in seed.options.misc)
                        Add_Options_Text(seed_rtb_3, String.Format("  {1,3} - {0}{2}", option.Key, option.Value ? "ON" : "OFF", Environment.NewLine), option.Value);
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Add_Options_Text(RichTextBox rtb, string text, bool enabled)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = enabled ? Color.DarkGreen : Color.Red;
            rtb.AppendText(text);
            rtb.SelectionColor = rtb.ForeColor;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void weap_toggle_totals_button_Click(object? sender, EventArgs? e)
        {
            weap_sc.Panel1Collapsed = !weap_sc.Panel1Collapsed;
            weap_toggle_totals_button.Text = weap_sc.Panel1Collapsed ? "Show Totals" : "Hide Totals";
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void item_toggle_totals_button_Click(object? sender, EventArgs e)
        {
            item_sc.Panel1Collapsed = !item_sc.Panel1Collapsed;
            item_toggle_totals_button.Text = item_sc.Panel1Collapsed ? "Show Totals" : "Hide Totals";
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void Show_Weapon_Type_button_Click(object sender, EventArgs e)
        {
            Button clicked_button = (Button)sender;

            if (clicked_button.BackColor != selected_highlite_color && clicked_button != weap_toggle_totals_button)
            {
                foreach (Control c in weap_panel.Controls)
                    if (c.GetType() == typeof(Button) && c != weap_toggle_totals_button)
                        c.BackColor = c == clicked_button ? selected_highlite_color : SystemColors.Control;

                Show_Selected_Weapon_Type();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void Show_Item_Button_Click(object sender, EventArgs e)
        {
            Button clicked_button = (Button)sender;

            if (clicked_button.BackColor != selected_highlite_color && clicked_button != item_toggle_totals_button)
            {
                foreach (Control c in item_panel.Controls)
                    if (c.GetType() == typeof(Button) && c != item_toggle_totals_button)
                        c.BackColor = c == clicked_button ? selected_highlite_color : SystemColors.Control;

                Show_Selected_Item_Type();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void hint_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update_Hint_DGV();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        public void Update_Hint_DGV()
        {
            if (bl2 == null) return;

            hint_dgv.Rows.Clear();

            foreach (var loot in bl2.loots)
                if (loot.hint == hint_comboBox.Text)
                    foreach (var row in loot.Get_Hint_Rows(hint_found_type, Loot_Type.ANY, true))
                        hint_dgv.Rows.Add(row);

            hint_dgv.ClearSelection();
            hint_dgv.CurrentCell = null;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void hint_dgv_2_SelectionChanged(object sender, EventArgs e)
        {
            string type = hint_dgv_2.Rows[hint_dgv_2.CurrentRow.Index].Cells[0].Value.ToString();
            hint_comboBox.SelectedIndex = hint_comboBox.FindString(type);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void Found_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Name.Contains("hint"))
            {
                hint_found_type = (Found_Type)rb.Tag;
                Update_Hint_DGV();
            }
            else if (rb.Name.Contains("item"))
            {
                item_found_type = (Found_Type)rb.Tag;
                Show_Selected_Item_Type();
            }
            else if (rb.Name.Contains("weap"))
            {
                weap_found_type = (Found_Type)rb.Tag;
                Show_Selected_Weapon_Type();
            }
            else if (rb.Name.Contains("area"))
            {
                area_found_type = (Found_Type)rb.Tag;
                Show_MEO_For_Selected_Map();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------
        private void dgv_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataGridView dgv = sender as DataGridView;
                DataGridViewCell cell = dgv.SelectedCells[0];

                string full_hint_txt = dgv.CurrentRow.Cells[e.ColumnIndex].Value as String;

                if (full_hint_txt.StartsWith("Hint: "))
                {
                    string hint = full_hint_txt.Split("Hint: ", StringSplitOptions.RemoveEmptyEntries)[0];
                    string name = dgv.CurrentRow.Cells[e.ColumnIndex - 1].Value as String;

                    foreach (DataGridViewRow row in hint_dgv_2.Rows)
                        if (String.Compare(row.Cells[1].Value.ToString(), name, true) == 0)
                        {
                            tc.SelectedTab = hint_tp;
                            hint_dgv_2.Select();
                            row.Selected = true;
                            hint_dgv_2.FirstDisplayedScrollingRowIndex = hint_dgv_2.SelectedRows[0].Index;
                            hint_comboBox.SelectedIndex = hint_comboBox.FindString(hint);
                        }
                }
            }
        }
    }
}
