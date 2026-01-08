import { useState } from "react";
import type { DragEvent } from "react";
import axios from "axios";

export default function MergePdf() {
    const [files, setFiles] = useState<File[]>([]);
    const [loading, setLoading] = useState(false);

    const onDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        const dropped = Array.from(e.dataTransfer.files).filter(
            (f) => f.type === "application/pdf"
        );
        setFiles((prev) => [...prev, ...dropped]);
    };

    const handleSubmit = async () => {
        const formData = new FormData();
        files.forEach((f) => formData.append("files", f));

        setLoading(true);
        const res = await axios.post("/api/pdf/merge", formData, {
            responseType: "blob",
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
            <h2 className="mb-3">Merge PDF</h2>

            <div
                className="border border-2 border-dashed rounded p-5 text-center mb-3"
                onDragOver={(e) => e.preventDefault()}
                onDrop={onDrop}
            >
                <p className="mb-2">Drag & drop PDFs here</p>
                <input
                    type="file"
                    multiple
                    accept="application/pdf"
                    className="form-control"
                    onChange={(e) =>
                        setFiles(Array.from(e.target.files ?? []))
                    }
                />
            </div>

            <ul className="list-group mb-3">
                {files.map((f, i) => (
                    <li
                        key={i}
                        className="list-group-item d-flex justify-content-between"
                    >
                        {f.name}
                        <span className="badge bg-secondary">
                            {Math.round(f.size / 1024)} KB
                        </span>
                    </li>
                ))}
            </ul>

            <button
                className="btn btn-primary"
                disabled={files.length < 2 || loading}
                onClick={handleSubmit}
            >
                {loading ? "Merging..." : "Merge PDFs"}
            </button>
        </div>
    );
}