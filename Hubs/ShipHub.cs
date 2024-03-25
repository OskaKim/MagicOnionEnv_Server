using MagicOnion.Server.Hubs;
using System.Threading.Tasks;

namespace GameServer.Hubs {
    public class ShipHub : StreamingHubBase<IShipHub, IShipHubReceiver>, IShipHub {
        private IGroup room;
        private string userName;
        IInMemoryStorage<string> storage;

        public async Task LeaveAsync() {
            Broadcast(room).OnLeave(userName);
            await room.RemoveAsync(Context);
        }

        public Task SendMessageAsync(string userName, string message) {
            throw new System.NotImplementedException();
        }

        protected override ValueTask OnDisconnected() {
            BroadcastExceptSelf(room).OnLeave(userName);
            return CompletedTask;
        }

        async Task IShipHub.JoinAsync(string roomName, string userName) {
            (room, storage) = await Group.AddAsync(roomName, userName);
            Broadcast(room).OnJoin(userName);
            this.userName = userName;
        }
    }
}
