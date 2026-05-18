import { useState } from "react";

import {
    getExecutionPlan,
    getMetrics
} from "./api";

import ResultView from "./components/ResultView";

export default function App() {

    const [query, setQuery] = useState("");

    const [planData, setPlanData] = useState(null);

    const [metricsData, setMetricsData] = useState(null);

    const handleSubmit = async () => {

        try {

            const [plan, metrics] = await Promise.all([
                getExecutionPlan(query),
                getMetrics(query)
            ]);

            setPlanData(plan);

            setMetricsData(metrics);

        } catch (err) {

            console.error(err);
        }
    };

    return (

        <div style={{ padding: "20px" }}>

            <h1>Index Analyzer</h1>

            <textarea
                rows="4"
                cols="60"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Enter SQL query..."
            />

            <br />

            <button onClick={handleSubmit}>
                Analyze
            </button>

            <ResultView
                data={planData}
                metrics={metricsData}
            />

        </div>
    );
}