import Cookies from 'js-cookie';
import Vue from "vue";
import { AxiosResponse } from "axios";

function getUrl(controller: string): string {
        if (process.env.NODE_ENV === 'production') {
                return `/api/${controller}`;//发布
        }
        else {
                // return `http://192.168.1.2:8080/api/${controller}`;//发布API调试
                return `https://localhost:44305/${controller}`;//调试
        }
}

export function postResetTask(id: number): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Reset?id=" + id));
}

export function postResetTasks(ids: number[]): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Reset/List"), ids);
}

export function postCancelTasks(ids: number[]): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Cancel/List"), ids);
}

export function postDeleteTasks(ids: number[]): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Delete/List"), ids);
}
export function postCancelTask(id: number): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Cancel?id=" + id));
}
export function postCancelQueue(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Queue/Cancel"));
}
export function postPauseQueue(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Queue/Pause"));
}
export function postResumeQueue(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Queue/Resume"));
}
export function postStartQueue(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Queue/Start"));
}
export function postAddCodeTask(item: any): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Add/Code"), item);
}
export function postAddConcatTask(item: any): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Add/Concat"), item);
}
export function postAddCombineTask(item: any): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Add/Combine"), item);
}
export function postAddCompareTask(item: any): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Add/Compare"), item);
}
export function postAddCustomTask(item: any): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Task/Add/Custom"), item);
}
export function getQueueStatus(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .get(getUrl("Queue/Status"))
} 
export function getTask(id: number) {
        return Vue.axios
                .get(getUrl("Task?id=" + id))
}
export function getTaskList(status: number | null, skip: number | null, take: number | null): Promise<AxiosResponse<any>> {
        let url = "Task/List?";
        if (status) {
                url = url + `status=${status}&`
        }
        if (skip) {
                url = url + `skip=${skip}&`
        }
        if (take) {
                url = url + `take=${take}&`
        }
        return Vue.axios
                .get(getUrl(url))
}

export function getMediaInfo(name: string): Promise<AxiosResponse<any>> {
        return Vue.axios
                .get(getUrl("MediaInfo") + "?name=" + name)
}

export function getMediaNames(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .get(getUrl("File/List/Input"))
}

export function download(name: string): void {
        window.open(getUrl("File/Download?id=" + encodeURI(name)));
}

export function getMediaDetails(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .get(getUrl("File/List/Output"))
}

export function getPresets(type: number | null = null): Promise<AxiosResponse<any>> {
        return type ?
                Vue.axios
                        .get(getUrl("Preset/List?type=" + type))
                :
                Vue.axios
                        .get(getUrl("Preset/List"))
}
export function postAddOrUpdatePreset(name: string, type: number, args: any): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Preset/Add"), { name: name, type: type, arguments: args });
}


export function getFtpStatus(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .get(getUrl("File/Ftp/Status"))
}

export function postFtp(input: boolean, on: boolean): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("File/" + (input ? "Ftp/Input" : "Ftp/Output") + "/" + (on ? "On" : "Off")))
}

export function postDeletePreset(id: number): Promise<AxiosResponse<any>> {
        return Vue.axios
                .post(getUrl("Preset/Delete?id=" + id))
}

export function getLogs(type: string | null, taskId: number, from: string | null, to: string | null, skip: number, take: number): Promise<AxiosResponse<any>> {
        let url = `Log/List?skip=${skip}&take=${take}`;
        if (type) {
                url += `&type=${type}`;
        }
        if (from) {
                url += `&from=${from}`;
        }
        if (to) {
                url += `&to=${to}`;
        }
        if (taskId != 0) {
                url += `&taskId=${taskId}`;
        }
        return Vue.axios
                .get(getUrl(url))
}

export function getUploadUrl(): string {
        return getUrl("File/Upload");
}

export function getFormats(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .get(getUrl("Task/Formats"))
}

export function setHeader(): void {
        Vue.axios.defaults.headers.common['Authorization'] = Cookies.get("token");
}

export function getNeedToken(): Promise<AxiosResponse<any>> {
        return Vue.axios
                .get(getUrl("Token/Need"))
}

export function getCheckToken(token: string): Promise<AxiosResponse<any>> {
        return Vue.axios
                .get(getUrl("Token/Check?token=") + token)
}