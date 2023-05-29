using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Code.Utils.Story
{

    public delegate void OnTitleChange(string title);

    public class BaseScript
    {
        protected int tryCounter;
        private string title;

        public BaseScript(int maxTry = 1, string title = null)
        {
            this.tryCounter = maxTry;
            this.title = title;
        }

        public virtual bool RunScript()
        {
            this.ChangeTitle(this.title);
            Init();
            bool canAction = false;
            while (!IsError() && !(canAction = CanAction()))
            {
                Wait();
                if (--tryCounter == 0)
                {
                    break;
                }
            }
            if (canAction)
            {
                Action();
                if (IsCompleted())
                {
                    OnCompleted();
                    return chooseAndRunNextScript();
                }
                else
                {
                    OnFailed();
                    return false;
                }
            }
            return false;
        }

        protected virtual void Init()
        {
        }

        protected virtual void Wait()
        {
        }

        protected virtual void Action()
        {
        }

        protected virtual bool CanAction()
        {
            return true;
        }

        protected virtual bool IsCompleted()
        {
            return true;
        }

        protected virtual bool IsError()
        {
            return false;
        }

        protected virtual void OnCompleted()
        {
        }

        protected virtual void OnFailed()
        {

        }

        public virtual void OnTerminateOrPause()
        {
            this.OnFailed();
        }

        protected List<Candidate> _candidates = new List<Candidate>();
        protected List<BaseScript> _scripts = new List<BaseScript>();

        public BaseScript AddNext(BaseScript nextScript, Candidate choose)
        {
            this._candidates.Add(choose);
            this._scripts.Add(nextScript);
            return this;
        }

        public BaseScript AddNext(BaseScript nextScript)
        {
            this._candidates.Add(() => true);
            this._scripts.Add(nextScript);
            return this;
        }

        protected virtual bool chooseAndRunNextScript()
        {
            int index = 0;
            foreach (Candidate candidate in this._candidates)
            {
                if (candidate.Invoke())
                {
                    break;
                }
                ++index;
            }
            if (index != this._candidates.Count)
            {
                this._scripts[index].onTitleChange = this.onTitleChange;
                return this._scripts[index].RunScript();
            }
            else return this._candidates.Count == 0;
        }

        public OnTitleChange onTitleChange = (title) => {};

        protected void ChangeTitle(string t)
        {
            if (t != null)
            {
                this.onTitleChange.Invoke(t);
            }
        }
    }
}
