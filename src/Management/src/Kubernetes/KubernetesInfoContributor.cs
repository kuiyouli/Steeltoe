// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Steeltoe.Common.Kubernetes;
using Steeltoe.Management.Info;
using System.Collections.Generic;

namespace Steeltoe.Management.Kubernetes
{
    public class KubernetesInfoContributor : IInfoContributor
    {
        private IPodUtilities _podUtilities;

        public KubernetesInfoContributor(IPodUtilities podUtilities)
        {
            _podUtilities = podUtilities;
        }

        public void Contribute(IInfoBuilder builder)
        {
            var current = _podUtilities.GetCurrentPodAsync().GetAwaiter().GetResult();
            var details = new Dictionary<string, object>();
            if (current != null)
            {
                details.Add("inside", true);
                details.Add("namespace", current.Metadata.NamespaceProperty);
                details.Add("podName", current.Metadata.Name);
                details.Add("podIp", current.Status.PodIP);
                details.Add("serviceAccount", current.Spec.ServiceAccountName);
                details.Add("nodeName", current.Spec.NodeName);
                details.Add("hostIp", current.Status.HostIP);
            }
            else
            {
                details.Add("inside", false);
            }

            builder.WithInfo("kubernetes", details);
        }
    }
}
