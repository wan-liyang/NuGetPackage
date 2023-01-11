using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.Model
{
    public enum DefaultPermissionTypeEnum
    {
        project,
        workbooks,
        datasources,
        dataroles,
        lenses,
        flows,
        metrics,
        databases,
        tables
    }

    public enum ProjectContentPermission
    {
        ManagedByOwner,
        LockedToProject,
        LockedToProjectWithoutNested
    }
}
