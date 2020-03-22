using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.InGame.ExtendScript
{
    public class ExtendScriptGame : IClearable, IDisposable
    {

        // Fields
        private Account _account;
        private const string configurationsPath = @"Parameters\ExtendScript\";
        private const string configurationsFolder = @"Parameters\ExtendScript";
        private const string FileExtension = ".EScript";
        public string FilePath = "";

        // Constructor
        public ExtendScriptGame(Account account)
        {
            _account = account;
        }

        public bool CreateFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            // Ensure that the configuration directory is there
            Directory.CreateDirectory(configurationsFolder);

            if (File.Exists(configurationsPath + filename + FileExtension))
            {
                return false;
            }
            else
            {
                File.Create(configurationsPath + filename + FileExtension);
                return true;
            }
        }
        public bool DeleteFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            // Ensure that the configuration directory is there
            Directory.CreateDirectory(configurationsFolder);

            if (File.Exists(configurationsPath + filename + FileExtension))
            {
                File.Delete(configurationsPath + filename + FileExtension);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool EditValueInt(string filename, string name, int value)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            int nbtry = 0;
            int maxNbtry = 200;
            
            while (nbtry < maxNbtry)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        //Ditcionary contenant toute les variables et valeurs 
                        Dictionary<string, int> AllValueInt = new Dictionary<string, int>();
                        Dictionary<string, string> AllValueString = new Dictionary<string, string>();

                        int nbvariablesint = 0;
                        int nbvariablesstring = 0;

                        using (BinaryReader br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            //Search the amount of var int
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesint = 0;
                            }
                            //Search the amount of var string
                            try
                            {
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesstring = 0;
                            }

                            //for each var , take the value 
                            if (nbvariablesint > 0)
                            {
                                //byte c = br.ReadByte();
                                for (int i = 0; i < nbvariablesint; i++)
                                {
                                    string tmpVar = br.ReadString();
                                    int tmpValue = br.ReadInt32();
                                    AllValueInt.Add(tmpVar, tmpValue);
                                }
                            }
                            if (nbvariablesstring > 0)
                            {
                                //byte c = br.ReadByte();
                                for (int i = 0; i < nbvariablesstring; i++)
                                {
                                    string tmpVar = br.ReadString();
                                    string tmpValue = br.ReadString();
                                    AllValueString.Add(tmpVar, tmpValue);
                                }
                            }
                            br.Close();
                        }

                        // If does'nt exist add the variable 
                        if (!AllValueInt.ContainsKey(name))
                        {
                            AllValueInt.Add(name, value);
                            nbvariablesint++;
                        }

                        // _account.Logger.LogError("AI", "Ecriture du fichier.");
                        using (BinaryWriter bw = new BinaryWriter(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                        {
                            bw.Write(nbvariablesint);
                            bw.Write(nbvariablesstring);

                            foreach (var obj in AllValueInt)
                            {
                                if (obj.Key == name)    //if the variable already exist just change value 
                                {
                                    bw.Write(name);
                                    bw.Write(value);
                                }
                                else
                                {
                                    bw.Write(obj.Key);
                                    bw.Write(obj.Value);
                                }
                            }

                            //write the list of var 
                            foreach (var obj in AllValueString)
                            {
                                bw.Write(obj.Key);
                                bw.Write(obj.Value);
                            }

                            bw.Close();
                        }
                        return true;
                    }
                }
                catch
                {
                    Task.Delay(500);
                    nbtry++;
                }
            }
            return true;
        }
        public bool EditValueString(string filename, string name, string value)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            int nbtry = 0;
            int maxNbtry = 200;

            while (nbtry < maxNbtry)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        //Ditcionary contenant toute les variables et valeurs 
                        Dictionary<string, int> AllValueInt = new Dictionary<string, int>();
                        Dictionary<string, string> AllValueString = new Dictionary<string, string>();

                        int nbvariablesint = 0;
                        int nbvariablesstring = 0;

                        using (BinaryReader br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {

                            //Search the amount of var
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                            }
                            catch (Exception e)
                            {
                                nbvariablesint = 0;
                            }
                            try
                            {
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch (Exception e)
                            {
                                nbvariablesstring = 0;
                            }

                            //for each var , take the value 
                            if (nbvariablesint > 0)
                            {
                                //byte c = br.ReadByte();
                                for (int i = 0; i < nbvariablesint; i++)
                                {
                                    string tmpVar = br.ReadString();
                                    int tmpValue = br.ReadInt32();
                                    AllValueInt.Add(tmpVar, tmpValue);
                                }
                            }

                            if (nbvariablesstring > 0)
                            {
                                //byte c = br.ReadByte();
                                for (int i = 0; i < nbvariablesstring; i++)
                                {
                                    string tmpVar = br.ReadString();
                                    string tmpValue = br.ReadString();
                                    AllValueString.Add(tmpVar, tmpValue);
                                }
                            }
                            br.Close();
                        }

                        // If does'nt exist add the variable 
                        if (!AllValueString.ContainsKey(name))
                        {
                            AllValueString.Add(name, value);
                            nbvariablesstring++;
                        }

                        // _account.Logger.LogError("AI", "Ecriture du fichier.");
                        using (BinaryWriter bw = new BinaryWriter(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                        {
                            bw.Write(nbvariablesint);
                            bw.Write(nbvariablesstring);

                            foreach (var obj in AllValueInt)
                            {
                                bw.Write(obj.Key);
                                bw.Write(obj.Value);
                            }

                            //write the list of var 
                            foreach (var obj in AllValueString)
                            {
                                if (obj.Key == name)    //if the variable already exist just change value 
                                {
                                    bw.Write(name);
                                    bw.Write(value);
                                }
                                else
                                {
                                    bw.Write(obj.Key);
                                    bw.Write(obj.Value);
                                }
                            }
                            bw.Close();
                        }
                        return true;
                    }
                }
                catch
                {
                    Task.Delay(500);
                    nbtry++;
                }
            }
        return true;
        }
        public bool DeleteVariable(string filename, string name)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            int nbtry = 0;
            int maxNbtry = 200;

            while (nbtry < maxNbtry)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        //Ditcionary contenant toute les variables et valeurs 
                        Dictionary<string, int> AllValueInt = new Dictionary<string, int>();
                        Dictionary<string, string> AllValueString = new Dictionary<string, string>();

                        int nbvariablesint = 0;
                        int nbvariablesstring = 0;

                        using (BinaryReader br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {

                            //Search the amount of var
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                            }
                            catch (Exception e)
                            {
                                nbvariablesint = 0;
                            }
                            try
                            {
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch (Exception e)
                            {
                                nbvariablesstring = 0;
                            }

                            //for each var , take the value 
                            if (nbvariablesint > 0)
                            {
                                //byte c = br.ReadByte();
                                for (int i = 0; i < nbvariablesint; i++)
                                {
                                    string tmpVar = br.ReadString();
                                    int tmpValue = br.ReadInt32();
                                    if (tmpVar != name)
                                    {
                                        AllValueInt.Add(tmpVar, tmpValue);
                                    }
                                    else
                                    {
                                        nbvariablesint--;
                                    }
                                }
                            }

                            if (nbvariablesstring > 0)
                            {
                                //byte c = br.ReadByte();
                                for (int i = 0; i < nbvariablesstring; i++)
                                {
                                    string tmpVar = br.ReadString();
                                    string tmpValue = br.ReadString();
                                    if (tmpVar != name)
                                    {
                                         AllValueString.Add(tmpVar, tmpValue);
                                    }
                                    else
                                    {
                                        nbvariablesstring--;
                                    }
                                }
                            }
                            br.Close();
                        }

                        // _account.Logger.LogError("AI", "Ecriture du fichier.");
                        using (BinaryWriter bw = new BinaryWriter(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                        {
                            bw.Write(nbvariablesint);
                            bw.Write(nbvariablesstring);

                            foreach (var obj in AllValueInt)
                            {
                                bw.Write(obj.Key);
                                bw.Write(obj.Value);
                            }

                            //write the list of var 
                            foreach (var obj in AllValueString)
                            {
                                bw.Write(obj.Key);
                                bw.Write(obj.Value);
                            }

                            bw.Close();
                        }
                        return true;
                    }
                }
                catch
                {
                    Task.Delay(500);
                    nbtry++;
                }
            }
            return true;
        }

         public int GetValueInt(string filename , string name)
        {
            if (string.IsNullOrEmpty(filename))
                return 0;

            int nbtry = 0;
            int maxNbtry = 200;

            while (nbtry < maxNbtry)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        int nbvariablesint = 0;
                        int nbvariablesstring = 0;

                        using (BinaryReader br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            //Get nb int var 
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                            }
                            catch 
                            {
                                 nbvariablesint = 0;
                            }
                            //Get nb string var 
                            try
                            {
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesstring = 0;
                            }

                            if (nbvariablesint > 0)
                            {
                                for (int i = 0; i < nbvariablesint; i++)
                                {
                                    string tmpvalname = br.ReadString();
                                    int tmpval = br.ReadInt32();
                                    if (tmpvalname == name)
                                    {
                                        br.Close();
                                        return tmpval;
                                    }
                                }
                            }
                            br.Close();
                        }
                        return 0;
                    }
                }
                catch
                {
                    Task.Delay(700);
                    nbtry++;
                }
            }
            return 0;
        }

        public string GetValueString(string filename, string name)
        {
            if (string.IsNullOrEmpty(filename))
                return "";
            int nbtry = 0;
            int maxNbtry = 200;

            while (nbtry < maxNbtry)
            {
                try
                {
                    if (File.Exists(configurationsPath + filename + FileExtension))
                    {
                        int nbvariablesint = 0;
                        int nbvariablesstring = 0;

                        using (BinaryReader br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            //Get nb int var 
                            try
                            {
                                nbvariablesint = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesint = 0;
                            }
                            //Get nb string var 
                            try
                            {
                                nbvariablesstring = br.ReadInt32();
                            }
                            catch
                            {
                                nbvariablesstring = 0;
                            }

                            if (nbvariablesint > 0)
                            {
                                for (int i = 0; i < nbvariablesint; i++)
                                {
                                    br.ReadString();
                                    br.ReadInt32();
                                }
                            }

                            if (nbvariablesstring > 0)
                            {
                                for (int i = 0; i < nbvariablesstring; i++)
                                {
                                    string tmpvalname = br.ReadString();
                                    string tmpval = br.ReadString();
                                    if (tmpvalname == name)
                                    {
                                        br.Close();
                                        return tmpval;
                                    }
                                }
                            }
                            br.Close();
                        }
                        return "";
                    }
                }
                catch
                {
                    Task.Delay(500);
                    nbtry++;
                }
            }
            return "";
        }

        public void Clear()
        {

        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                _account = null;
               // _FileSemaphore = null;

                disposedValue = true;
            }
        }

        ~ExtendScriptGame() => Dispose(false);

        public void Dispose() => Dispose(true);

        #endregion
    }
}
