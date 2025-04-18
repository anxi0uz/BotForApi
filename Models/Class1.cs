using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bot.Models
{
    public enum StudentState 
    {
        AwaitingPostFio,
        AwaitingPostIdSpec,
        AwaitingPostBirhtday,
        AwaitingPostUchebnoeZav,
        AwaitingPostPhoneNumber,
        AwaitingPostAdress,
        AwaitingPostAge,
        AwaitingFinalPost,
        AwaitingGetId,
        AwaitingUpdateFio,
        AwaitingUpdateIdSpec,
        AwaitingUpdateBirhtday,
        AwaitingUpdateUchebnoeZav,
        AwaitingUpdatePhoneNumber,
        AwaitingUpdateAdress,
        AwaitingUpdateAge,
        AwaitingDeleteById,
    }
}
