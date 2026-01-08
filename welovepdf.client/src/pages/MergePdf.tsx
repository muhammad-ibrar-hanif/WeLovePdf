import { useState } from "react";
import axios from "axios";


export default function MergePdf() {
    const [files, setFiles] = useState<File[]>([]);
    const [loading, setLoading] = useState(false);


    const handleSubmit = async () => {
        const formData = new FormData();
        files.forEach(f => formData.append("files", f));


        setLoading(true);
        const res = await axios.post("/api/pdf/merge", formData, {
            responseType: "blob"
        });
        setLoading(false);


        const url = window.URL.createObjectURL(res.data);
        const a = document.createElement("a");
        a.href = url;
        a.download = "merged.pdf";
        a.click();
    };


    return (
        <div className="container py-5">
            <h2>Merge PDF</h2>
            <input
                type="file"
                multiple
                accept="application/pdf"
                className="form-control mb-3"
                onChange={e => setFiles(Array.from(e.target.files || []))}
            />
            <button
                className="btn btn-primary"
                disabled={files.length < 2 || loading}
                onClick={handleSubmit}
            >
                {loading ? "Merging..." : "Merge"}
            </button>
        </div>
    );
}