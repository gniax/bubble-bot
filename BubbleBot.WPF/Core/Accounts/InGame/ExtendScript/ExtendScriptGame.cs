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
        public static SemaphoreSlim _FileSemaphore = new SemaphoreSlim(1, 1);
        public string FilePath = "";
        public Dictionary<string, int> FileData { get; set; }


        // Constructor
        public ExtendScriptGame(Account account)
        {
            _account = account;
            FileData = new Dictionary<string, int>();
        }

        public bool CreateFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            _FileSemaphore.Wait();

            // Ensure that the configuration directory is there
            Directory.CreateDirectory(configurationsFolder);

            if (File.Exists(configurationsPath + filename + FileExtension))
            {
                _FileSemaphore.Release();
                return false;
            }
            else
            {
                File.Create(configurationsPath + filename + FileExtension);
                _FileSemaphore.Release();
                return true;
            }
        }
        public bool DeleteFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            _FileSemaphore.Wait();

            // Ensure that the configuration directory is there
            Directory.CreateDirectory(configurationsFolder);

            if (File.Exists(configurationsPath + filename + FileExtension))
            {
                File.Delete(configurationsPath + filename + FileExtension);
                _FileSemaphore.Release();
                return true;
            }
            else
            {
                _FileSemaphore.Release();
                return false;
            }
        }
        public bool EditValue(string filename, string name,int value)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }
               
            //_account.Logger.LogError("AI", "On entre dans la fonction." + configurationsPath + filename + FileExtension + name + value.ToString());
            _FileSemaphore.Wait();

            if (File.Exists(configurationsPath + filename + FileExtension))
            {
                //Ditcionary contenant toute les variables et valeurs 
                Dictionary<string, int> AllValue = new Dictionary<string, int>();
                int nbvariables = 0;
                //Recupère les infos dans le fichier 
                //_account.Logger.LogError("AI", "Lecture du fichier.");
                
                using (BinaryReader br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                   // _account.Logger.LogError("AI", "File open , cherche nbvar.");
                    try
                    {
                        nbvariables = br.ReadInt32();
                    }
                    catch (Exception e)
                    {
                       // _account.Logger.LogError("AI", "File open , Erreur lors de la recherche du nombre de variables.");
                    }
                    //_account.Logger.LogError("AI", "File open , nbvar." + nbvariables.ToString());
                    if (nbvariables > 0)
                    {
                        //byte c = br.ReadByte();
                        for (int i = 0; i < nbvariables; i++)
                        {
                            string tmpVar = br.ReadString();
                            int tmpValue = br.ReadInt32();

                            AllValue.Add(tmpVar, tmpValue);
                        }
                    }
                    br.Close();
                }
               // _account.Logger.LogError("AI", "Add value to dictionary.");
                if (!AllValue.ContainsKey(name))
                {
                    AllValue.Add(name, value);
                    nbvariables++;
                }
               // _account.Logger.LogError("AI", "Ecriture du fichier.");
                using (BinaryWriter bw = new BinaryWriter(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    bw.Write(nbvariables);

                        foreach(var obj  in AllValue)
                        {
                            if (obj.Key == name)
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
            }
            _FileSemaphore.Release();
            return true;
        }
        public bool LoadFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }
                

            _FileSemaphore.Wait();

            if (File.Exists(configurationsPath + filename + FileExtension))
            {
                int nbvariables = 0;
                FileData = new Dictionary<string, int>();

                using (BinaryReader br = new BinaryReader(File.Open(configurationsPath + filename + FileExtension, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    try
                    {
                        nbvariables = br.ReadInt32();
                    }
                    catch (Exception e)
                    {
                        // _account.Logger.LogError("EScript", "Not var found.");
                    }
                    if (nbvariables > 0)
                    {
                        FileData.Add("NB_VARIABLE", nbvariables);
                        for (int i = 0; i < nbvariables; i++)
                        {
                            FileData.Add(br.ReadString(), br.ReadInt32());
                        }
                    }
                    br.Close();
                }
            }
            else
            {
                _FileSemaphore.Release();
               // ActionFinished();
                return false;
            }

            _FileSemaphore.Release();
            //ActionFinished();
            return true;
        }
       public int GetValue(string name)
       {
           foreach (var obj in FileData)
           {
               if (name == obj.Key)
               {
                    return obj.Value;
               }
           }
            return 0;
       }
        private void ActionFinished()
        {
            _account.Scripts.ActionsManager.ActionFinishedBeforeCoroutine = true;
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

                disposedValue = true;
            }
        }

        ~ExtendScriptGame() => Dispose(false);

        public void Dispose() => Dispose(true);

        #endregion
    }
}
