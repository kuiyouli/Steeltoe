﻿// Copyright 2017 the original author or authors.
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

using OpenTelemetry.Trace;
using Steeltoe.Extensions.Logging;
using Steeltoe.Management.OpenTelemetry.Trace;
using System.Text;

namespace Steeltoe.Management.Tracing
{
    public class TracingLogProcessor : IDynamicMessageProcessor
    {
        private readonly ITracing tracing;
        private readonly ITracingOptions options;

        public TracingLogProcessor(ITracingOptions options, ITracing tracing)
        {
            this.options = options;
            this.tracing = tracing;
        }

        public string Process(string inputLogMessage)
        {
            var currentSpan = GetCurrentSpan();
            if (currentSpan != null)
            {
                var context = currentSpan.Context;

                var sb = new StringBuilder(" [");
                sb.Append(options.Name);
                sb.Append(",");

                var traceId = context.TraceId.ToHexString();
                if (traceId.Length > 16 && options.UseShortTraceIds)
                {
                    traceId = traceId.Substring(traceId.Length - 16, 16);
                }

                sb.Append(traceId);
                sb.Append(",");

                sb.Append(context.SpanId.ToHexString());
                sb.Append(",");

                // TODO: Currently OTel doesnt support getting parentId from Active spans.
                /*
                //if (currentSpan.ParentSpanId != null)
                //{
                //    sb.Append(currentSpan.ParentSpanId.ToLowerBase16());
                //}
                //
                //sb.Append(",");
                */

                if (currentSpan.IsRecording)
                {
                    sb.Append("true");
                }
                else
                {
                    sb.Append("false");
                }

                sb.Append("] ");
                return sb.ToString() + inputLogMessage;
            }

            return inputLogMessage;
        }

        protected internal TelemetrySpan GetCurrentSpan()
        {
            var span = tracing.Tracer?.CurrentSpan;
            if (span == null || !span.Context.IsValid)
            {
                return null;
            }

            return span;
        }
    }
}
