using System;
using System.Diagnostics;

namespace Stas.GA {
    public abstract partial class aTasker {
        DateTime next_m_use; //last mana flask used
        public void UseManaFlask() {
            var mana_cost = ui.sett.mana_cast_price;
            if (ui.worker != null)
                mana_cost = ui.worker.main.mana_cost;
            var low_mana = ui.life.Mana.Current < mana_cost;
            var can_use = DateTime.Now > next_m_use;
            var auto_mana = ui.danger > 0 && ui.worker.b_mana_use_auto;
            if (can_use &&( low_mana || auto_mana)) {
                var mkey = ui.sett.mana_flask_key;
                if (ui.worker != null)
                    mkey = ui.worker.mana_flask_key;
                Keyboard.KeyPress(mkey, tName+ ".UseManaFlask", true);
                var add_time = ui.sett.mana_flask_ms;
                if (ui.worker != null)
                    add_time = ui.worker.mana_flask_ms;
                next_m_use = DateTime.Now.AddMilliseconds(add_time);
            }
        }
        DateTime next_l_use;//next life flask use
        public void UseLifeFlask(bool dont_check = false) { //dont_check =>if bot use if from leader comand
            var use_lf = ui.sett.b_use_life_flask;
            if (ui.life == null)
                return;
            var low_life = ui.life.Health.CurrentInPercent < ui.sett.trigger_life_left_persent;
            if (ui.worker!=null)
                low_life=ui.life.Health.CurrentInPercent < ui.worker.min_life_percent;
            bool can_use = DateTime.Now > next_l_use;
            if (dont_check || (low_life && can_use)) {
                var key = ui.sett.life_flask_key;
                if (ui.worker != null)
                    key = ui.worker.life_flask_key;
                Keyboard.KeyPress(key, tName + "UseLifeFlask");
                var add_time = ui.sett.life_flask_ms;
                if (ui.worker != null)
                    add_time = ui.worker.life_flask_ms;
                next_l_use = DateTime.Now.AddMilliseconds(add_time);
            }
        }
    }
}
