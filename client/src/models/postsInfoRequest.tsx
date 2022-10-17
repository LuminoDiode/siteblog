import { throws } from "assert";

export enum postType{
    any = "any",
    domainNews = "domain",
    personalPosts ="personal"
}

export default class postsInfoRequest {
    postsType?:string|postType = "any";

    public constructor(postsType:string|postType){
        this.postsType=postsType;
    }
}