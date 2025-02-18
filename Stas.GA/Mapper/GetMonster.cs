﻿using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;
namespace Stas.GA; 

public partial class AreaInstance {
    MapItem GetMonster(Entity e, MapItem mi) {
        if (e.GetComp<ObjectMagicProperties>(out var _omp)) {
            var cond_one = e.eType == eTypes.Stage0FIT && _omp.Rarity != Rarity.Unique;
            if (cond_one) {
                return null;
            }
        }

        if (e.IsHidden) {
            mi.uv = sh.GetUV(MapIconsIndex.hidden);
            return mi;
        }
        e.GetComp<NPC>(out var _npc);
        e.GetComp<Life>(out var _life);
        var bad_npc = _npc != null && _npc.IsIgnoreHidden;
        if (_life == null || bad_npc) //|| !e.IsTargetable
            return null;
        if (e.IsFriendly) {
            if (e.Metadata.Contains("BlightTower")) {
                //mi.uv = sh.GetUV(MapIconsIndex.BlightTowerStune);
                //mi.priority = IconPriority.Low;
                //return mi;
                return null;
            }
            //if (ui.worker.b_use_balistas) { 
            //}
            //if (!e.Path.Contains("Summoned")) { 
            //}
            frame_party.Add(e);
            if (ui.sett.b_show_frendly_mobs) {
                mi.uv = sh.GetUV(MapIconsIndex.SmallWhiteCircle);
                mi.priority = IconPriority.Low;
                return mi;
            }
            return null;
        }
        else {
            if (e.rarity == Rarity.Unique && e.Path.Contains("Metadata/Monsters/Spirit/"))
                mi.uv = sh.GetUV(MapIconsIndex.Spirit);

            if (e.GetComp<Buffs>(out var buffs)) { //onslaught MonsterModApplyShock
                if (buffs.StatusEffects.ContainsKey("proximity_shield_aura")) {  //archnemesis_bubble_shield 
                    mi.uv2 = sh.GetUV(MapIconsIndex.ProximityShield);
                    mi.size = 32;
                    mi.priority = IconPriority.Double;
                }
                else if (buffs.StatusEffects.ContainsKey("visual_archnemesis_mod_display_buff")) {
                    mi.uv2 = sh.GetUV(MapIconsIndex.Aura_life_es_leech);
                    mi.size = 32;
                    mi.priority = IconPriority.Double;
                }
            }

            if (_omp != null && _omp.Mods != null) {
                if (_omp.Mods.Contains("MonsterModShakariTouched")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RedBoss);
                    mi.priority = IconPriority.Critical;
                    mi.size = 26;
                    return mi;
                }
                else if (_omp.Mods.Any(m => m.Contains("BestiaryMod"))) {
                    mi.size = Math.Clamp(mi.size + 4 + (_omp.Mods.Count - 4), 16, 25);
                    mi.priority = IconPriority.VeryHigh;
                    if (e.rarity == Rarity.Normal) {
                        mi.uv = sh.GetUV(MapIconsIndex.BestiaryBloodAlter);
                    }
                    else if (e.rarity == Rarity.Magic) {
                        mi.uv = sh.GetUV(MapIconsIndex.BestiaryBlueMonster);
                    }
                    else if (e.rarity == Rarity.Rare) {
                        mi.uv = sh.GetUV(MapIconsIndex.BestiaryYellowBeast);
                    }
                    else if (e.rarity == Rarity.Unique) {
                        mi.uv = sh.GetUV(MapIconsIndex.BestiaryBoss);
                    }
                }
                if (_omp.Mods.Count > 2) {
                }
                if (_omp.Mods.Any(m => m.Contains("ArchnemesisFrostTouched")))
                    mi.uv = sh.GetUV(MapIconsIndex.frost);
                else if (_omp.Mods.Any(m => m.Contains("MonsterArchnemesisVolatileFlameBlood")))
                    mi.uv = sh.GetUV(MapIconsIndex.FlameBlood);
                else if (_omp.Mods.Any(m => m.Contains("HeraldOfTheObelisk") || m.Contains("LivingCrystals")))
                    mi.uv = sh.GetUV(MapIconsIndex.Heralding_minions);
                else if (_omp.Mods.Contains("FlameWalker"))
                    mi.uv = sh.GetUV(MapIconsIndex.BestiaryBoss);
                else if (_omp.Mods.Contains("MonsterMapBoss"))
                    mi.uv = sh.GetUV(MapIconsIndex.BestiaryBoss);
                else if (_omp.Mods.Any(a => a.Contains("MonsterAura") || a.Contains("CannotBeStunned") || a.Contains("MonsterFastRun")))
                    mi.uv = sh.GetUV(MapIconsIndex.AuraFasterRuner);
                else if (_omp.Mods.Any(a => a.Contains("Bloodlines") || a.Contains("CorruptedBlood")))
                    mi.uv = sh.GetUV(MapIconsIndex.bleed);
                else if (e.RenderName.Contains("Vampiric"))
                    mi.uv = sh.GetUV(MapIconsIndex.Vampiric);
                else if (e.RenderName.Contains("Cavestalker"))
                    mi.uv = sh.GetUV(MapIconsIndex.Cavestalker);
            }
        }
        return mi;//MonsterIcon
    }
}
