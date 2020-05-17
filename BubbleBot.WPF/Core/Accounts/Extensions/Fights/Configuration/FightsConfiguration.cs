using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Extensions.Fights.Configuration.Enums;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BubbleBot.Core.Accounts.Extensions.Fights.Configuration
{
    public class FightsConfiguration : ViewModelBase, IDisposable
    {
        // Fields
        public const string ConfigurationsPath = @"Parameters\Fights";
        private Account _account;
        private bool _approachWhenNoSpellWasCasted;
        private bool _baseApproachOnAllMonsters;
        private BlockSpectatorScenarios _blockSpectatorScenario;
        private FightSpeeds _fightsSpeed;
        private FightStartPlacements _fightStartPlacements;
        private bool _ignoreSommonedEnnemies;
        private bool _loaded;
        private bool _lockFight;
        private byte _maxCells;
        private int _monsterToApproach;
        private byte _regenEnd;
        private byte _regenStart;
        private int _spellToApproach;
        private FightTactics _tactic;


        // Constructor
        public FightsConfiguration(Account account)
        {
            _account = account;
            FightStartPlacement = FightStartPlacements.FAR_FROM_ENNEMIS;
            MonsterToApproach = -1;
            SpellToApproach = -1;
            BlockSpectatorScenario = BlockSpectatorScenarios.NEVER;
            LockFight = false;
            Tactic = FightTactics.FUGITIVE;
            MaxCells = 12;
            ApproachWhenNoSpellWasCasted = false;
            BaseApproachOnAllMonsters = false;
            RegenStart = 0;
            RegenEnd = 100;
            Spells = new ObservableCollection<Spell>();
            FightsSpeed = FightSpeeds.NORMAL;
        }


        // Properties
        private string ConfigFilePath => Path.Combine(ConfigurationsPath,
            LanguageManager.Translate("68", _account.AccountConfig.Username));


        // Properties
        public FightStartPlacements FightStartPlacement
        {
            get => _fightStartPlacements;
            set
            {
                Set(ref _fightStartPlacements, value);
                Save();
            }
        }

        public int MonsterToApproach
        {
            get => _monsterToApproach;
            set
            {
                Set(ref _monsterToApproach, value);
                Save();
            }
        }

        public int SpellToApproach
        {
            get => _spellToApproach;
            set
            {
                Set(ref _spellToApproach, value);
                Save();
            }
        }

        public BlockSpectatorScenarios BlockSpectatorScenario
        {
            get => _blockSpectatorScenario;
            set
            {
                Set(ref _blockSpectatorScenario, value);
                Save();
            }
        }

        public bool LockFight
        {
            get => _lockFight;
            set
            {
                Set(ref _lockFight, value);
                Save();
            }
        }

        public FightTactics Tactic
        {
            get => _tactic;
            set
            {
                Set(ref _tactic, value);
                Save();
            }
        }

        public byte MaxCells
        {
            get => _maxCells;
            set
            {
                Set(ref _maxCells, value);
                Save();
            }
        }

        public bool ApproachWhenNoSpellWasCasted
        {
            get => _approachWhenNoSpellWasCasted;
            set
            {
                Set(ref _approachWhenNoSpellWasCasted, value);
                Save();
            }
        }

        public bool BaseApproachOnAllMonsters
        {
            get => _baseApproachOnAllMonsters;
            set
            {
                Set(ref _baseApproachOnAllMonsters, value);
                Save();
            }
        }

        public byte RegenStart
        {
            get => _regenStart;
            set
            {
                Set(ref _regenStart, value);
                Save();
            }
        }

        public byte RegenEnd
        {
            get => _regenEnd;
            set
            {
                Set(ref _regenEnd, value);
                Save();
            }
        }

        [JsonProperty("Spells")]
        public ObservableCollection<Spell> Spells { get; private set; }

        public bool IgnoreSummonedEnnemies
        {
            get => _ignoreSommonedEnnemies;
            set
            {
                Set(ref _ignoreSommonedEnnemies, value);
                Save();
            }
        }

        public FightSpeeds FightsSpeed
        {
            get => _fightsSpeed;
            set
            {
                Set(ref _fightsSpeed, value);
                Save();
            }
        }


        public void Load()
        {
            _loaded = false;

            if (File.Exists(ConfigFilePath))
                try
                {
                    using (var sr = new StreamReader(File.Open(ConfigFilePath, FileMode.Open, FileAccess.ReadWrite,
                        FileShare.ReadWrite), Encoding.UTF8))
                    {
                        var json = JObject.Parse(sr.ReadToEnd());

                        FightStartPlacement = (FightStartPlacements) json.SelectToken("FightStartPlacement").Value<byte>();
                        BlockSpectatorScenario = (BlockSpectatorScenarios) json.SelectToken("BlockSpectatorScenario").Value<byte>();
                        Tactic = (FightTactics) json.SelectToken("Tactic").Value<byte>();
                        FightsSpeed = (FightSpeeds) json.SelectToken("FightsSpeed").Value<byte>();
                        MonsterToApproach = json.SelectToken("MonsterToApproach").Value<int>();
                        SpellToApproach = json.SelectToken("SpellToApproach").Value<int>();
                        LockFight = json.SelectToken("LockFight").Value<bool>();
                        MaxCells = json.SelectToken("MaxCells").Value<byte>();
                        ApproachWhenNoSpellWasCasted = json.SelectToken("ApproachWhenNoSpellWasCasted").Value<bool>();
                        BaseApproachOnAllMonsters = json.SelectToken("BaseApproachOnAllMonsters").Value<bool>();
                        RegenStart = json.SelectToken("RegenStart").Value<byte>();
                        RegenEnd = json.SelectToken("RegenEnd").Value<byte>();
                        IgnoreSummonedEnnemies = json.SelectToken("IgnoreSummonedEnnemies").Value<bool>();

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Spells.Clear();
                            var value = json["Spells"];
                            var spells = value.ToObject<List<Spell>>();
                            foreach (var spell in spells)
                            {
                                if (!Spells.Contains(spell))
                                {
                                    Spells.Add(spell);
                                }
                            }
                        });

                    }
                }
                catch
                {
                    Console.WriteLine("Error while loading fights configuration '{0}'.", ConfigFilePath);
                }

            _loaded = true;
        }

        public void Save()
        {
            // Avoid saving when we're loading
            if (!_loaded)
                return;

            // Ensure that the configuration directory is there
            Directory.CreateDirectory(ConfigurationsPath);

            using (var sw = new StreamWriter(File.Open(ConfigFilePath, FileMode.Create, FileAccess.ReadWrite,
                FileShare.ReadWrite), Encoding.UTF8))
            {
                dynamic json = new ExpandoObject();

                json.FightStartPlacement = (byte) FightStartPlacement;
                json.BlockSpectatorScenario = (byte) BlockSpectatorScenario;
                json.Tactic = (byte) Tactic;
                json.FightsSpeed = (byte) FightsSpeed;
                json.MonsterToApproach = MonsterToApproach;
                json.SpellToApproach = SpellToApproach;
                json.LockFight = LockFight;
                json.MaxCells = MaxCells;
                json.ApproachWhenNoSpellWasCasted = ApproachWhenNoSpellWasCasted;
                json.BaseApproachOnAllMonsters = BaseApproachOnAllMonsters;
                json.RegenStart = RegenStart;
                json.RegenEnd = RegenEnd;
                json.IgnoreSummonedEnnemies = IgnoreSummonedEnnemies;
                json.Spells = Spells;

                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(sw, json);
            }
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                Spells = null;
                _account = null;

                _disposedValue = true;
            }
        }

        ~FightsConfiguration()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}