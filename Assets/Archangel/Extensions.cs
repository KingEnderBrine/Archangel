using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archangel
{
    public static class Extensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return dict[key] = defaultValue;
        }

        public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultValueFunc)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return dict[key] = defaultValueFunc == null ? default : defaultValueFunc();
        }

        public static void ClaimButtonPressAuthority(this ISkillState skillState, SkillLocator skillLocator, InputBankTest inputBank)
        {
            var activatorSkillSlot = skillState.activatorSkillSlot;
            if (!skillLocator || !activatorSkillSlot || !inputBank)
            {
                return;
            }

            switch (skillLocator.FindSkillSlot(activatorSkillSlot))
            {
                case SkillSlot.None:
                    break;
                case SkillSlot.Primary:
                    inputBank.skill1.hasPressBeenClaimed = true;
                    break;
                case SkillSlot.Secondary:
                    inputBank.skill2.hasPressBeenClaimed = true;
                    break;
                case SkillSlot.Utility:
                    inputBank.skill3.hasPressBeenClaimed = true;
                    break;
                case SkillSlot.Special:
                    inputBank.skill4.hasPressBeenClaimed = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
