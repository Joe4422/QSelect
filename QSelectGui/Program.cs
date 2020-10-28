using System;
using System.Collections.Generic;
using Terminal.Gui;
using LibQSelect;
using System.Linq;

namespace QSelectGui
{
    class Program
    {
        protected static List<Mod> inactiveMods;
        protected static QSelect select;
        protected static ListView inactiveModListView;
        protected static ListView activeModListView;
        protected static Mod id1Mod;

        static void Main(string[] args)
        {
            // Load LibQSelect content
            select = new QSelect("config.json");

            inactiveMods = select.Mods.Where((x) => select.EnabledMods.Contains(x) == false).ToList();
            id1Mod = select.Mods.Where((x) => x.Directory == "id1").First();

            MakeModActive(id1Mod);

            // Init application
            Application.Init();
            var top = Application.Top;

            // Init binary selector
            Window binaryWindow = new Window("Binary")
            {
                Width = Dim.Percent(40.0f),
                Height = Dim.Fill()
            };
            top.Add(binaryWindow);
            ListView binaryListView = new ListView(select.Binaries)
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            binaryWindow.Add(binaryListView);
            binaryListView.SelectedItemChanged += BinaryListView_SelectedItemChanged;
            binaryListView.OpenSelectedItem += BinaryListView_OpenSelectedItem;
            binaryListView.SelectedItem = select.Binaries.IndexOf(select.SelectedBinary);

            // Init mod selector
            Window inactiveModWindow = new Window("Inactive Mods")
            {
                X = Pos.Right(binaryWindow),
                Width = Dim.Percent(60.0f),
                Height = Dim.Percent(50.0f)
            };
            top.Add(inactiveModWindow);
            inactiveModListView = new ListView(inactiveMods)
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            inactiveModWindow.Add(inactiveModListView);
            inactiveModListView.OpenSelectedItem += InactiveModListView_OpenSelectedItem;
            Window activeModWindow = new Window("Active Mods")
            {
                X = Pos.Right(binaryWindow),
                Y = Pos.Bottom(inactiveModWindow),
                Width = Dim.Percent(60.0f),
                Height = Dim.Percent(50.0f)
            };
            top.Add(activeModWindow);
            activeModListView = new ListView(select.EnabledMods)
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(1)
            };
            activeModWindow.Add(activeModListView);
            activeModListView.OpenSelectedItem += ActiveModListView_OpenSelectedItem;
            Button moveModUpButton = new Button("^")
            {
                Width = 5,
                Height = 1,
                Y = Pos.Bottom(activeModListView)
            };
            Button moveModDownButton = new Button("v")
            {
                Width = 5,
                Height = 1,
                Y = Pos.Bottom(activeModListView),
                X = Pos.Right(moveModUpButton)
            };
            activeModWindow.Add(moveModUpButton);
            activeModWindow.Add(moveModDownButton);
            moveModUpButton.Clicked += MoveModUpButton_Clicked;
            moveModDownButton.Clicked += MoveModDownButton_Clicked;

            Application.Run();
        }

        private static void BinaryListView_SelectedItemChanged(ListViewItemEventArgs obj)
        {
            select.Settings.LastBinary = (obj.Value as Binary).Directory;
        }

        private static void MoveModDownButton_Clicked()
        {
            if (activeModListView.SelectedItem > 0 && activeModListView.SelectedItem < select.EnabledMods.Count - 1)
            {
                Mod mod = select.EnabledMods[activeModListView.SelectedItem];

                select.MoveInLoadOrder(mod, 1);
                activeModListView.MoveDown();
            }

            activeModListView.SetFocus();
        }

        private static void MoveModUpButton_Clicked()
        {
            if (activeModListView.SelectedItem > 1)
            {
                Mod mod = select.EnabledMods[activeModListView.SelectedItem];

                select.MoveInLoadOrder(mod, -1);
                activeModListView.MoveUp();
            }

            activeModListView.SetFocus();
        }

        private static void BinaryListView_OpenSelectedItem(ListViewItemEventArgs obj)
        {
            if (select.EnabledMods.Count > 0)
            {
                Binary binary = (Binary)obj.Value;

                select.SelectedBinary = binary;

                select.RunBinary();
            }
        }

        private static void ActiveModListView_OpenSelectedItem(ListViewItemEventArgs obj)
        {
            Mod mod = (Mod)obj.Value;

            if (mod != id1Mod) MakeModInactive(mod);
        }

        private static void InactiveModListView_OpenSelectedItem(ListViewItemEventArgs obj)
        {
            Mod mod = (Mod)obj.Value;

            MakeModActive(mod);
        }

        private static void MakeModActive(Mod mod)
        {
            inactiveMods.Remove(mod);
            select.EnableMod(mod);

            //mod.Dependencies.ForEach((x) => MakeModActive(x));
        }

        private static void MakeModInactive(Mod mod)
        {
            select.DisableMod(mod);
            if (inactiveMods.Contains(mod) == false) inactiveMods.Add(mod);

            //mod.Dependencies.ForEach((x) => MakeModInactive(x));
        }
    }
}
