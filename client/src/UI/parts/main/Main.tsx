import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Route, Routes } from "react-router-dom";
import { postType } from "src/models/postsInfoRequest";
import PostsList from "src/UI/components/auth/postList/PostsList";
import PostCard from "src/UI/components/postCard/PostCard";
import classes from "./Main.module.css";
import PostEditor from "src/UI/components/PostEditor/PostEditor";

const Main = () => {
    return (
        <div role={"main"} className={classes.appMain}>
            <Routes>
                <Route path="news" element={ <PostsList postsType={postType.domainNews} /> } />
                <Route path="blog" element={ <PostsList postsType={postType.personalPosts} /> } />
                <Route path="services" element={<span>Coming soon!</span>} />
                <Route path="post/" element={<span>Coming soon!</span>} />
                <Route path="post/create" element={<PostEditor/>}/>
                <Route path="post/:postId" element={<span>Coming soon!</span>} />
            </Routes>
        </div>
    );
}

export default Main;