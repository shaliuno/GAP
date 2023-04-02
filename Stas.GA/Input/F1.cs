using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;
public partial class InputChecker {

    void F1() {
        if (Keyboard.b_Try_press_key(Keys.F1, "Input checker", 500, true)) {
            //ui.sett.b_debug = true;
            if (ui.b_alt) {
                //ui.test.uiElementFinder();
                //ui.test.GetTopElemUnderCursor();
                ui.test.GetRootElemUnderCursor(1);
                //ui.test.WorlToSPCheck();
            }
            else {
                ui.test_elem = ui.gui.map_root;
                #region OLD
                //var cam = ui.m.Read<CameraOffsets>( ui.camera.Address);
                //ui.test.FindUiElemNotUnick("Nessa");
                //ui.test_elem =FindSameUIElement("Decorations", 1)[0].Item1; //660
                //ui.test_elem =FindSameUIElement("Inventory",1)[0].Item1; //0x568
                //ui.test_elem = ui.gui.large_map;
                //var elem = new Element("test");
                //ui.texts.Clear();
                //elem.Tick(new nint(0x234DE439790));
                //GetEntComponetns();
                //ui.me.GetComp<Positioned>(out var pos);
                //var skill = ui.worker.jump.skill.CanBeUsed;
                //ui.test_elem = ui.gui;
                //var stats = ui.me.Stats;
                //b_busy_info_crash();
                //var b = ui.gui.b_busy;
                //var bi = ui.gui.b_busy_info;
                //ui.test_elem = ui.gui;
                //CompareActor();
                //var ea = ui.gc?.entity_list.Entities.OrderBy(x => x.gdist_to_me).ToList();
                //ui.test_elem = ui.gui.chat_box_elem;
                //var mess = ui.gui.chat_box_elem.messages;
                //ui.test_elem = ui.gui.player_inventory;
                //ui.test.Inventry();
                //ui.test.GetElemUnderCursor(2); //540
                //ui.test.CheckBuffType();
                //ui.test.Render();
                //var old_skill = old_actor.ActorSkills[10];

                //var sw = skill.EffectsPerLevel.SkillGemWrapper; 
                //var bufs = ui.me.buffs;
                //ui.me.GetComp<Actor>(out var actor);
                //var skills = actor.actor_skills;
                //TestSkill();//+
                //var curr_map_name = ui.curr_map_name;
                //var cmn2 = ui.states.area_loading_state.CurrentAreaName;
                #endregion
            }
        }
    }
   
    void TestCamera() {
        var ptr = ui.camera.Address;
        var co = new CameraOffsets();
        Camera.GetCameraOffsets(ptr, ui.states.ingame_state.Address, ref co);
    }
   
    void b_busy_info_crash() {
        var tl = new List<int>();
        int i = 0;
        for (; ; ) {
            var check = ui.b_busy;
            var value = ui.gui.b_busy_info;
            var info = "i=[" + (i++) + "] size=[" + value.Length + "] v=" + value;
            tl.Add(value.Length);
            ui.AddToLog(info);
        }
    }
    //void ComareCam() {
    //    var old_cam = ui_loader.ui.gc.IngameState.Camera;
    //    var cam = ui.camera;
    //    Debug.Assert(cam.Address == old_cam.ptr);
    //}
    //void CompareActor() {
    //    ui.me.GetComp<Actor>(out var actor);
    //    ui_loader.ui.me.GetComp<ExileCore.PoEMemory.Components.Actor>(out var old_actor);
    //    for (int i = 8; i < old_actor.ActorSkills.Count; i++) {
    //        var os = old_actor.ActorSkills[i];//old
    //        var cs = actor.actor_skills[i];//current
    //        Debug.Assert(os.ptr == cs.Address);
    //        Debug.Assert(os.EffectsPerLevel.ptr == cs.EffectsPerLevel.Address);
    //        Debug.Assert(os.EffectsPerLevel.SkillGemWrapper.ActiveSkill.ptr
    //                        == cs.EffectsPerLevel.SkillGemWrapper.ActiveSkill.Address);
    //    }
    //}
}