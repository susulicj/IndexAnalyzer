import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    Tooltip,
    CartesianGrid,
    ResponsiveContainer
} from "recharts";

export default function MetricsChart({ metrics }) {

    const data = [
        {
            name: "Logical Reads",
            value: metrics.logicalReads
        },
        {
            name: "Physical Reads",
            value: metrics.physicalReads
        },
        {
            name: "CPU Time",
            value: metrics.cpuTimeMs
        },
        {
            name: "Elapsed Time",
            value: metrics.elapsedTimeMs
        },
        {
            name: "Scan Count",
            value: metrics.scanCount
        },
        {
            name: "Read Ahead",
            value: metrics.readAheadReads
        }
    ];

    return (

        <div style={{ width: "100%", height: 400 }}>

            <ResponsiveContainer>

                <BarChart data={data}>

                    <CartesianGrid strokeDasharray="3 3" />

                    <XAxis dataKey="name" />

                    <YAxis />

                    <Tooltip />

                    <Bar dataKey="value" />

                </BarChart>

            </ResponsiveContainer>

        </div>
    );
}