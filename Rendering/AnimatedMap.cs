using Rendering.LogHook;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Rendering
{
    public partial class AnimatedMap : Form
    {
        public AnimatedMap() {
            InitializeComponent();
            this.debugWindow.Hide();
            this.ClientSize = new System.Drawing.Size(861, 861);
        }

        private Timer tmr = new Timer();
        private void LoadEvent(object sender, EventArgs e) {
            tmr.Interval = 100;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Start();
            string mainChar = Microsoft.VisualBasic.Interaction.InputBox("Player name?", "Player Character Select", "N/A");
            LogHook.EntityStateMaster.Instance.SetMainCharacter(mainChar);
            // doubt this is how you're supposed to do it but xdd
            StartHook();
        }
        private void tmr_Tick(object sender, EventArgs e) {
            // todo: calculate new positions for lasers
            var debug = LogHook.EntityStateMaster.Instance.DebugEntityPositions();
            debugWindow.Text = debug;
            this.Invalidate();
        }
        private void DrawEvent(object sender, PaintEventArgs e) {
            var state = LogHook.EntityStateMaster.Instance;
            var playersToRender = state.GetPlayersToRender();
            var worldMarkersToRender = state.GetWorldMarkersToRender();
            var debuffDropLocationsToRender = state.GetIndicatorsToRender();
            var creaturesToRender = state.GetCreaturesToRender();
            var linesFromCreaturesToDraw = state.GetBeamsFromCreaturesToRender();
            var minX = state.MinXPos; var minY = state.MinYPos;
            var maxX = state.MaxXPos; var maxY = state.MaxYPos;

            foreach (var lineFromCreature in linesFromCreaturesToDraw) {
                DrawLineFromEntityInFacingDirection(lineFromCreature.entity, lineFromCreature.beam, e, minX, maxX, minY, maxY);
            }

            foreach (var creature in creaturesToRender) {
                DrawCreature(creature, e, minX, maxX, minY, maxY);
            }

            foreach (var indicatorEntity in debuffDropLocationsToRender) {
                DrawIndicator(indicatorEntity, e, minX, maxX, minY, maxY);
            }

            foreach (var playerEntity in playersToRender) {
                DrawPlayer(playerEntity, e, minX, maxX, minY, maxY);
            }

            foreach (var worldMarkerEntity in worldMarkersToRender) {
                DrawWorldMarker(worldMarkerEntity.X, worldMarkerEntity.Y, worldMarkerEntity.RenderIdentifier, worldMarkerEntity.IsOnField, e, minX, maxX, minY, maxY);
            }

        }
        private void DrawIndicator(LogHook.Entity indicator, PaintEventArgs e, float minX, float maxX, float minY, float maxY) {
            if (!indicator.IsOnField) return;

            float x1 = ((maxX - indicator.X) / (maxX - minX)) * 800;
            float y1 = ((maxY - indicator.Y) / (maxY - minY)) * 800;

            var graphics = e.Graphics;

            if (indicator.IsHighlighted) {

                var highLightPath = new System.Drawing.Drawing2D.GraphicsPath();
                highLightPath.AddEllipse(x1 - 20, y1 - 20, 40, 40);
                var highLightRegion = new Region(highLightPath);

                var h = indicator.HighlightColour;
                var highlightBrush = new SolidBrush(Color.FromArgb(180, h.R, h.G, h.B));
                graphics.FillRegion(highlightBrush, highLightRegion);
            }

        }

        private void DrawCreature(LogHook.Entity creature, PaintEventArgs e, float minX, float maxX, float minY, float maxY) {
            if (!creature.IsOnField) return;

            float x1 = ((maxX - creature.X) / (maxX - minX)) * 800;
            float y1 = ((maxY - creature.Y) / (maxY - minY)) * 800;
            //Console.WriteLine(creature.X + "/" + creature.Y + " | " + minX + "-" + maxX + " | " + minY + "-" + maxY + " | " + x1 + "/" + y1);

            var graphics = e.Graphics;
            var image = Properties.Resources.boss;
            graphics.DrawImage(image, x1 - 25, y1 - 25, 50, 50);

        }

        private void DrawPlayer(LogHook.Entity player, PaintEventArgs e, float minX, float maxX, float minY, float maxY) {
            if (!player.IsOnField) return;

            float x1 = ((maxX - player.X) / (maxX - minX)) * 800;
            float y1 = ((maxY - player.Y) / (maxY - minY)) * 800;

            var graphics = e.Graphics;

            if(player.IsHighlighted) {

                var highLightPath= new System.Drawing.Drawing2D.GraphicsPath();
                highLightPath.AddEllipse(x1 - 20, y1 - 20, 40, 40);
                var highLightRegion = new Region(highLightPath);

                var h = player.HighlightColour;
                var highlightBrush = new SolidBrush(Color.FromArgb(180, h.R, h.G, h.B));
                graphics.FillRegion(highlightBrush, highLightRegion);
            }
            if (player.RenderIdentifier == "Main") {
                var playerImage = Properties.Resources.boss;
                //playerImage.RotateFlip()
                graphics.DrawImage(playerImage, x1 - 15, y1 - 15, 30, 30);
            } else {
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddEllipse(x1 - 10, y1 - 10, 20, 20);
                var region = new Region(path);
                // change colour with class colour
                var c = GetRGBFromPlayerClass(player.RenderIdentifier);
                var brush = new SolidBrush(Color.FromArgb(255, c.R, c.G, c.B));
                graphics.FillRegion(brush, region);
            }


        }

        private (int R, int G, int B) GetRGBFromPlayerClass(string playerClass) {
            switch (playerClass) {
                case "Death Knight":
                    return (196, 30, 58);
                case "Demon Hunter":
                    return (163, 48, 201);
                case "Druid":
                    return (255, 124, 10);
                case "Evoker":
                    return (51, 147, 127);
                case "Hunter":
                    return (170, 211, 114);
                case "Mage":
                    return (63, 199, 234);
                case "Monk":
                    return (0, 255, 152);
                case "Paladin":
                    return (244, 140, 186);
                case "Priest":
                    return (255, 255, 255);
                case "Rogue":
                    return (255, 244, 104);
                case "Shaman":
                    return (0, 112, 221);
                case "Warlock":
                    return (135, 136, 238);
                case "Warrior":
                    return (198, 155, 109);
                default:
                    return (153, 153, 153);
                    //throw new Exception($"unhandled class {playerClass}");
            }
        }

        private void DrawWorldMarker(float x, float y, string markerIdentifier, bool onField, PaintEventArgs e, float minX, float maxX, float minY, float maxY) {
            if (!onField) return;

            // todo: change with icons
            //var path = new System.Drawing.Drawing2D.GraphicsPath();
            float x1 = ((maxX - x) / (maxX - minX)) * 800;
            float y1 = ((maxY - y) / (maxY - minY)) * 800;
            //path.AddEllipse(x1, y1, 10, 10);
            //var region = new Region(path);
            var graphics = e.Graphics;
            var image = GetImageFromWorldMarkerName(markerIdentifier);
            graphics.DrawImage(image, x1-15, y1-15, 30, 30);
            //graphics.FillRegion(Brushes.White, region);

        }

        private Image GetImageFromWorldMarkerName(string worldMarkerName) {
            switch (worldMarkerName) {
                case "blue":
                    return Properties.Resources.blue;
                case "green":
                    return Properties.Resources.green;
                case "purple":
                    return Properties.Resources.purple;
                case "red":
                    return Properties.Resources.red;
                case "yellow":
                    return Properties.Resources.yellow;
                case "orange":
                    return Properties.Resources.orange;
                case "silver":
                    return Properties.Resources.silver;
                case "skull":
                    return Properties.Resources.skull;
                default:
                    throw new Exception($"unknown marker name {worldMarkerName}");
            }
        }
        private void DrawLineFromEntityInFacingDirection(LogHook.Entity entity, LogHook.BeamEntity beam, PaintEventArgs e, float minX, float maxX, float minY, float maxY) {
            float x1 = ((maxX - entity.X) / (maxX - minX)) * 800;
            float y1 = ((maxY - entity.Y) / (maxY - minY)) * 800;
            float x2 = x1 + beam.Length * (float)Math.Cos(entity.Rotation);
            float y2 = y1 - beam.Length * (float)Math.Sin(entity.Rotation);

            var c = beam.Colour;
            var width = beam.Width;
            var pen = new Pen(Color.FromArgb(255, c.R, c.G, c.B), width);
            var graphics = e.Graphics;
            graphics.DrawLine(pen, x1-width/2, y1-width/2, x2-width/2, y2-width/2);
        }

        private void StartHook() {
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "WoW Combat Logs (WoWCombatLog*.txt)|WoWCombatLog*.txt|*.*|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;


                Process wow = Process.GetProcessesByName("Wow").FirstOrDefault();
                if (wow != null) {
                    Console.WriteLine(wow.MainModule.ModuleName);
                    Console.WriteLine(wow.MainModule.FileName.Replace(wow.MainModule.ModuleName, "logs"));
                    openFileDialog.InitialDirectory = wow.MainModule.FileName.Replace(wow.MainModule.ModuleName, "logs");
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    RunHookAsync(filePath);
                } else
                {
                    Application.Exit();
                }
            }
        }

        private async void RunHookAsync(String filePath) {
            await Task.Run(() => {
                LogHook.LogHook.ReadFile(filePath);
            });
        }

    }
}
