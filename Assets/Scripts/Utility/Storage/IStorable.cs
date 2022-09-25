namespace Strid.Utility.Storage {
    /// <summary>Can be stored. Possesses a distinct ID.</summary>
    public interface IStorable {
        void SetId(int id);
        
        int GetId();
    }
}