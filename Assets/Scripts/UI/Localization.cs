using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UI {
    [UsedImplicitly]
    public class Localization {
        private enum Language {
            En,
            Ru,
        }

        private readonly Dictionary<Language, Dictionary<string, string>> library = new Dictionary<Language, Dictionary<string, string>>();
        
        public Localization() {
            const Language en = Language.En;
            const Language ru = Language.Ru;
            library[en] = new Dictionary<string, string>();
            library[ru] = new Dictionary<string, string>();

            //=================================================================================================================
            // English library
            //=================================================================================================================
            library[en].Add("sec", "sec.");

            library[en].Add("acquiredGlyph_blinkCost", "Acquired a blink cost glyph.");
            library[en].Add("acquiredGlyph_blinkPrecision", "Acquired a blink precision glyph.");
            library[en].Add("sign_arenaAndGolem", "This way: Test fight arena and an old HostileGolem");
            library[en].Add("sign_enemyDemoRooms", "This way: Enemy demo rooms");
            library[en].Add("sign_corruptedFires", "This way: Corrupted Fire demo room");
            library[en].Add("sign_eternalGuardians", "This way: Eternal Guardian demo room");
            library[en].Add("sign_crystalRangers", "This way: Crystal Ranger weird demo room");
            library[en].Add("chair_joke", "You wanted to sit on a chair... until you realized you have no body.");
            library[en].Add("error_soulEquipCooldown", "Soul re-equipping is on cooldown: ");
            library[en].Add("soulEquipped_knight", "Knight's soul equipped");
            library[en].Add("soulEquipped_berserker", "Berserker's soul equipped");
            library[en].Add("soulEquipped_marksman", "Marksman's soul equipped");
            library[en].Add("soulEquipped_priestess", "Priestess's soul equipped");
            library[en].Add("soulEquipped_shieldmaiden", "Shieldmaiden's soul equipped");
            library[en].Add("error_noTarget", "No target selected");

            //=================================================================================================================
            // Russian library
            //=================================================================================================================
            library[ru].Add("sec", "с.");

            library[ru].Add("acquiredGlyph_blinkCost", "Получен знак эффективности рывка");
            library[ru].Add("acquiredGlyph_blinkPrecision", "Получен знак точности рывка");
            library[ru].Add("sign_arenaAndGolem", "Здесь находятся тестовая арена и путь к старому нерабочему голему.");
            library[ru].Add("sign_enemyDemoRooms", "Демонстрационные комнаты");
            library[ru].Add("sign_corruptedFires", "Осквернённые Огни или Лень Ставить Стены.");
            library[ru].Add("sign_eternalGuardians", "Вечные Стражи или Квадратная Комната.");
            library[ru].Add("sign_crystalRangers", "Кристальные Стрелки или Гениальный Дизайн Уровней.");
            library[ru].Add("chair_joke", "Вам хотелось присесть на стул... а потом оказалось, что садиться нечем.");
            library[ru].Add("error_soulEquipCooldown", "Смена активного духа будет доступна через ");
            library[ru].Add("soulEquipped_knight", "Экипирован дух рыцаря");
            library[ru].Add("soulEquipped_berserker", "Экипирован дух берсерка");
            library[ru].Add("soulEquipped_marksman", "Экипирован дух стрелка");
            library[ru].Add("soulEquipped_priestess", "Экипирован дух жрицы");
            library[ru].Add("error_noTarget", "Нет цели");
        }

        private static Language GetCurrentLanguage() {
            return Language.En;
        }
	
        public string Get(string key) {
            var currentLanguage = GetCurrentLanguage();
            string localizedString;
            if (library[currentLanguage].TryGetValue(key, out localizedString)) {
                return localizedString;
            }

            if (library[Language.En].TryGetValue(key, out localizedString)) {
                return localizedString;
            }

            return "";
        }
    }
}
