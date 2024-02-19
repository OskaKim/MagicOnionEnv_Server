using MagicOnion.Server.Hubs;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Hubs
{
    public class CharaMoveHub : StreamingHubBase<ICharaMoveHub, ICharaMoveHubReceiver>, ICharaMoveHub
    {
        private IGroup room;
        private string userName;
        IInMemoryStorage<string> storage;

        /// <summary>
        /// 入室通知
        /// </summary>
        /// <param name="roomName">ルーム名</param>
        /// <param name="userName">ユーザー名</param>
        /// <returns></returns>
        public async Task<string[]> JoinAsync(string roomName, string userName)
        {
            (room, storage) = await Group.AddAsync(roomName, userName);
            Broadcast(room).OnJoin(userName);
            this.userName = userName;
            return storage.AllValues.ToArray();
        }

        /// <summary>
        /// 退室通知
        /// </summary>
        /// <returns></returns>
        public async Task LeaveAsync()
        {
            await room.RemoveAsync(Context);
            Broadcast(room).OnLeave(userName);
        }

        /// <summary>
        /// 切断通知
        /// </summary>
        /// <returns></returns>
        protected override ValueTask OnDisconnected()
        {
            BroadcastExceptSelf(room).OnLeave(userName);
            return CompletedTask;
        }
    }
}
