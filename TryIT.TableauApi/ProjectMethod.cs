using System.Net.Http;
using System.Threading.Tasks;
using TryIT.TableauApi.ApiResponse;
using TryIT.TableauApi.Model;

namespace TryIT.TableauApi
{
    public partial class TableauConnector
    {/// <summary>
     /// get project by name and parent project id, if <paramref name="parentProjectId"/> is null or empty, means get top level project
     /// </summary>
     /// <param name="parentProjectId"></param>
     /// <param name="projectName">values are case sensitive, refer to https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes</param>
     /// <returns></returns>
        public async Task<SiteModel.Project> GetProject(string parentProjectId, string projectName)
        {
            projectName = projectName.Replace(" ", "+");
            string url = string.Empty;
            if (string.IsNullOrEmpty(parentProjectId))
            {
                url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects?filter=topLevelProject:eq:true,name:eq:{projectName}";
            }
            else
            {
                url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects?filter=parentProjectId:eq:{parentProjectId},name:eq:{projectName}";
            }
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetProjectResponse.Response>();
            if (result.projects.project == null)
            {
                return null;
            }
            return result.projects.project[0].ToProject();
        }

        /// <summary>
        /// create a project under <paramref name="parentProjectId"/>, if <paramref name="parentProjectId"/> is null or empty, will create as top level project
        /// </summary>
        /// <param name="parentProjectId"></param>
        /// <param name="projectName"></param>
        /// <param name="projectDescription"></param>
        /// <param name="projectContentPermission">This value controls user permissions in a project, default is ManagedByOwner</param>
        /// <returns></returns>
        public async Task<SiteModel.Project> CreateProject(string parentProjectId, string projectName, string projectDescription, ProjectContentPermission projectContentPermission = ProjectContentPermission.ManagedByOwner)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects";
            string request = $"<tsRequest><project parentProjectId=\"{parentProjectId}\" name=\"{projectName}\" description=\"{projectDescription}\" contentPermissions=\"{projectContentPermission}\" /></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await RestApiInstance.PostAsync(url, requestContent);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<CreateProjectResponse.Response>();

            if (result.project == null)
            {
                return null;
            }

            return result.project.ToProject();
        }

        /// <summary>
        /// Updates the name, description, or project hierarchy of the specified project. You can create or update project hierarchies by specifying a parent project for the project that you are updating using this method.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="newParentProjectId"></param>
        /// <param name="newProjectName"></param>
        /// <param name="newProjectDescription"></param>
        /// <param name="newProjectContentPermission"></param>
        /// <returns></returns>
        public async Task<SiteModel.Project> UpdateProject(string projectId, string newParentProjectId, string newProjectName, string newProjectDescription, ProjectContentPermission newProjectContentPermission = ProjectContentPermission.ManagedByOwner)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects/{projectId}";
            string request = $"<tsRequest><project parentProjectId=\"{newParentProjectId}\" name=\"{newProjectName}\" description=\"{newProjectDescription}\" contentPermissions=\"{newProjectContentPermission}\" /></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await RestApiInstance.PutAsync(url, requestContent);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<CreateProjectResponse.Response>();

            if (result.project == null)
            {
                return null;
            }

            return result.project.ToProject();
        }
    }
}
