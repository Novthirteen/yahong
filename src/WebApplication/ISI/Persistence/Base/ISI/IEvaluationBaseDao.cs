using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IEvaluationBaseDao
    {
        #region Method Created By CodeSmith

        void CreateEvaluation(Evaluation entity);

        Evaluation LoadEvaluation(String userCode);
  
        IList<Evaluation> GetAllEvaluation();
  
        IList<Evaluation> GetAllEvaluation(bool includeInactive);
  
        void UpdateEvaluation(Evaluation entity);
        
        void DeleteEvaluation(String userCode);
    
        void DeleteEvaluation(Evaluation entity);
    
        void DeleteEvaluation(IList<String> pkList);
    
        void DeleteEvaluation(IList<Evaluation> entityList);    
        #endregion Method Created By CodeSmith
    }
}
