import axios from "axios";

const BASE_URL = "https://localhost:7193/api/Analyzer";

export const getExecutionPlan = async (query) => {
    const response = await axios.post(
        `${BASE_URL}/execution-plan`,
        {
            query: query
        }
    );

    return response.data;
};

export const getMetrics = async (query) => {
    const response = await axios.post(
        `${BASE_URL}/metrics`,
        {
            query: query,
            tableName: "View",
            useIndex: false,
            indexName: null
        }
    );

    return response.data;
};