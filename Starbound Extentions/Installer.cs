using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace Starbound_Extensions
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                string targetdir = Context.Parameters["targetdir"];
                string name = "Starbound Extensions.exe";

                string path = "\"" + targetdir + name + "\"";
                //set the .pak file
                Registry.SetValue(@"HKEY_CLASSES_ROOT\SystemFileAssociations\.pak\shell\Unpack\command", null, path + " \"%1\"");

                //set the .player file
                Registry.SetValue(@"HKEY_CLASSES_ROOT\SystemFileAssociations\.player\shell\Make versioned json\command", null, path + " \"%1\"");

                //set the .json file
                Registry.SetValue(@"HKEY_CLASSES_ROOT\SystemFileAssociations\.json\shell\Dump versioned json\command", null, path + " \"%1\"");

                //set the pack folder option
                Registry.SetValue(@"HKEY_CLASSES_ROOT\Directory\shell\Pack\command", null, path + " \"%V\"");
            }
            catch (Exception)
            {
                base.Rollback(savedState);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            Registry.ClassesRoot.DeleteSubKeyTree(@"SystemFileAssociations\.pak");
            Registry.ClassesRoot.DeleteSubKeyTree(@"SystemFileAssociations\.player");
            Registry.ClassesRoot.DeleteSubKeyTree(@"SystemFileAssociations\.json");
            Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\shell\Pack");
        }
    }
}
